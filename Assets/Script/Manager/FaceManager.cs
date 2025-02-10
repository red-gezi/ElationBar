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
    public Chara currentChara=>GetComponent<PlayerManager>().currentPlayerChara;

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
        string log = $"new(Chara.{transform.parent.name}, new() {{" + string.Join(", ", blendShadpeID) + "}}),";
        Debug.Log(log);
    }
    //设置人物表情
    [Button("设置人物表情")]
    public async void SetFace(int index)
    {
        ResetFace();
        Duration = 5;
        var targetFaceDatas = GameData.FaceDatas
            .Where(data => data.CurrentChara == currentChara).ToList();
        if (targetFaceDatas.Count > index)
        {
            var targetFaceData = targetFaceDatas[index];
            await CustomThread.TimerAsync(0.3f, progress =>
            {
                for (int i = 0; i < targetFaceData.KeyIndex.Count; i++)
                {
                    skinnedMeshRenderer.SetBlendShapeWeight(targetFaceData.KeyIndex[i], Mathf.Lerp(0, 100, progress));
                }
            });
        }
        else
        {
            Debug.LogWarning("无法找到对应表情数据");
        }
    }
    [Button("重置表情")]
    async void ResetFace()
    {
        //筛选出所有卡牌涉及到的keyid
        var targetIndex = GameData.FaceDatas
              .Where(data => data.CurrentChara == currentChara)
              .SelectMany(data => data.KeyIndex)
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
    public void SetVoice(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
    //通过网络同步其他玩家角色说台词
    public void SetVoice(int id)
    {
        var voices = AssetBundleManager
                .LoadAll<AudioClip>(currentChara.ToString(), "Voice")
                .Where(voice => voice.name.StartsWith("V"))
                .ToList();
        if (id <= voices.Count)
        {
            SetVoice(voices[id]);
        }
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
