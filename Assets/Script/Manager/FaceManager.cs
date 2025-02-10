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
    [Header("����ģ��")]
    public SkinnedMeshRenderer skinnedMeshRenderer = null;
    public Chara currentChara=>GetComponent<PlayerManager>().currentPlayerChara;

    public bool mute = true;
    public float gain = 1.0f;
    [Tooltip("�������ģ��")]
    [Range(1, 100)]
    private int smoothAmount = 70;
    [Header("��������")]
    [Header("��")]
    public int a;
    [Header("��")]
    public int e;
    [Header("��")]
    public int i;
    [Header("��")]
    public int o;
    [Header("��")]
    public int u;
    float Duration;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (skinnedMeshRenderer == null)
        {
            Debug.LogError("δ�������ģ��");
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
        //��������ͬ����ǩ
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
    [Button("��ӡ��������")]
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
    [Button("��ӡ��ǰ��������")]
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
    //�����������
    [Button("�����������")]
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
            Debug.LogWarning("�޷��ҵ���Ӧ��������");
        }
    }
    [Button("���ñ���")]
    async void ResetFace()
    {
        //ɸѡ�����п����漰����keyid
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
    //��������̨��
    public void SetVoice(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
    //ͨ������ͬ��������ҽ�ɫ˵̨��
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
    //��������ͬ������
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
