using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public class FaceManager : OVRLipSyncContextBase
   
{
    // Start is called before the first frame update
    [Header("人物模型")]
    public SkinnedMeshRenderer skinnedMeshRenderer = null;
    public Chara currentChara;
    List<FaceData> FaceDatas { get; set; } = new List<FaceData>();

    public bool mute = true;
    public float gain = 1.0f;
    [Tooltip("人物表情模型")]
    [Range(1, 100)]
    private int smoothAmount = 70;
    [Header("口型索引")]
    [Header("あ")]
    public int a;
    [Header("え")]
    public int e;
    [Header("い")]
    public int i;
    [Header("お")]
    public int o;
    [Header("う")]
    public int u;
    float Duration;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (skinnedMeshRenderer == null)
        {
            Debug.LogError("未添加人物模型");
            return;
        }
        else
        {
            Smoothing = smoothAmount;
        }
        AddFaceData();
    }
    void AddFaceData()
    {
        FaceDatas.Add(new(Chara.砂金, 0, new() { 8, 16, 19, 44, 58, 59 }));//喜
        FaceDatas.Add(new(Chara.砂金, 1, new() { 1, 8, 61 }));//怒
        FaceDatas.Add(new(Chara.砂金, 2, new() { 7, 56 }));//哀
        FaceDatas.Add(new(Chara.砂金, 3, new() { 6, 48, 50, 59 }));//惧
        FaceDatas.Add(new(Chara.黑天鹅, 0, new() { 13, 20, 51 }));//喜
        FaceDatas.Add(new(Chara.黑天鹅, 1, new() { 0, 1, 24, 37 }));//怒
        FaceDatas.Add(new(Chara.黑天鹅, 2, new() { 4, 9, 44, 67 }));//哀
        FaceDatas.Add(new(Chara.黑天鹅, 3, new() { 23, 34, 40 }));//惧
    }
    // Update is called once per frame
    void Update()
    {
        if (Duration > 0)
        {
            Duration -= Time.deltaTime;
            if (Duration <= 0)
            {
                ResetFace();
            }
        }
        //设置语音同步标签
        if ((skinnedMeshRenderer != null))
        {
            OVRLipSync.Frame frame = GetCurrentPhonemeFrame();
            if (frame != null)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(a, frame.Visemes[10] * 100.0f);
                skinnedMeshRenderer.SetBlendShapeWeight(e, frame.Visemes[11] * 100.0f);
                skinnedMeshRenderer.SetBlendShapeWeight(i, frame.Visemes[12] * 100.0f);
                skinnedMeshRenderer.SetBlendShapeWeight(o, frame.Visemes[13] * 100.0f);
                skinnedMeshRenderer.SetBlendShapeWeight(u, frame.Visemes[14] * 100.0f);
            }
            if (smoothAmount != Smoothing)
            {
                Smoothing = smoothAmount;
            }
        }
    }
    [Button("打印所有索引")]
    public void ShowFaceID()
    {
        List<string> blendShadpeNames = new List<string>();
        for (int i = 0; i < skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
        {
            string blendShadpeName = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i);
            blendShadpeNames.Add(blendShadpeName + $"_{i}");
        }
        Debug.Log(blendShadpeNames.ToJson());
    }
    [Button("打印当前表情索引")]
    public void ShowCurrentFaceID()
    {
        List<int> blendShadpeID = new();
        for (int i = 0; i < skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
        {
            if (skinnedMeshRenderer.GetBlendShapeWeight(i) != 0)
            {
                blendShadpeID.Add(i);
            }
        }
        Debug.Log(blendShadpeID.ToJson(Newtonsoft.Json.Formatting.None));
    }
    //设置人物表情
    [Button("设置人物表情")]
    public async void SetFace( int index)
    {
        ResetFace();
        Duration = 5;
        var targetFaceData = FaceDatas
            .FirstOrDefault(data => data.CurrentChara == currentChara && data.FaceIndex == index);
        await CustomThread.TimerAsync(0.3f, progress =>
        {
            for (int i = 0; i < targetFaceData.keyIndex.Count; i++)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(targetFaceData.keyIndex[i], Mathf.Lerp(0, 100, progress));
            }
        });
    }
    [Button("重置表情")]
    async void ResetFace()
    {
        //筛选出所有卡牌涉及到的keyid
        var targetIndex = FaceDatas
              .Where(data => data.CurrentChara == currentChara)
              .SelectMany(data => data.keyIndex)
              .Distinct()
              .ToList();
        var currentWeight = targetIndex
            .Select(index => skinnedMeshRenderer.GetBlendShapeWeight(index))
            .ToList();
        await CustomThread.TimerAsync(0.3f, progress =>
        {
            for (int i = 0; i < targetIndex.Count; i++)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(targetIndex[i], Mathf.Lerp(currentWeight[i], 0, progress));
            }
        });
    }
    //设置人物台词
    public void SetWord(Chara chara, int index)
    {

    }
    //根据语音同步表情
    void OnAudioFilterRead(float[] data, int channels)
    {
        if ((OVRLipSync.IsInitialized() != OVRLipSync.Result.Success) || audioSource == null)
        {
            return;
        }
        data = data.Select(x => x * gain).ToArray();
        lock (this)
        {
            if (Context == 0 || OVRLipSync.IsInitialized() != OVRLipSync.Result.Success)
            {
                return;
            }
            var frame = Frame;
            OVRLipSync.ProcessFrame(Context, data, frame, channels == 2);
        }
        if (!mute)
        {
            data = data.Select(x => x * 0.0f).ToArray();
        }
    }
}
class FaceData
{
    public Chara CurrentChara { get; set; }
    public int FaceIndex { get; set; }
    public Sprite CharaSprite { get; set; }
    public List<int> keyIndex = new();

    public FaceData(Chara currentChara, int faceIndex, List<int> keyIndex)
    {
        CurrentChara = currentChara;
        FaceIndex = faceIndex;
        this.keyIndex = keyIndex;
        //表情立绘从资源包加载
    }
}