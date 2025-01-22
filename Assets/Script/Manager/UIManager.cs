using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting.YamlDotNet.Core;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : GeziBehaviour<UIManager>
{
    //[Header("��¼")]
    //public TMP_InputField playerName;

    public CanveType CurrentCanveType { get; set; }
    [Header("�������")]
    public Canvas roomCanvas;
    public Text roomName;
    public Text roomPassword;

    public GameObject creatRoomUI;
    [Header("���ý���")]
    public Canvas configCanvas;

    public GameObject charaSelectListUI;
    public Text playerName;
    public Transform faceListOnConfig;
    public Transform voiceListOnConfig;
    public Transform voiceContentOnConfig;
    public Transform voiceItemOnConfiga;
    [Header("��Ϸ����")]
    public Canvas gameCanvas;
    public Transform faceListOnGame;
    public Transform voiceListOnGame;
    public GameObject timerUI;
    public GameObject playButtonUI;
    public GameObject questionButtonUI;

    [Header("����")]
    public Image microphoneImage;
    private void Start()
    {

    }
    //���̰�������
    //private void Update()
    //{
    //    switch (CurrentCanveType)
    //    {
    //        case CanveType.Room: break;
    //        case CanveType.Config:
    //            {
    //                if (IsActionListOpenOnConfig)
    //                {

    //                }
    //                if (IsFaceListOpenOnConfig)
    //                {
    //                    for (int i = 0; i < 4; i++)
    //                    {
    //                        if (Input.GetKeyDown(KeyCode.Alpha1+i)) Instance.SelectCharaFaceListOnConfig(i);
    //                    }
    //                }
    //                if (IsVoiceListOpenOnConfig)
    //                {
    //                    for (int i = 0; i<9;i++)
    //                    {
    //                        if (Input.GetKeyDown(KeyCode.Alpha1+i)) Instance.SelectCharaVoiceListOnConfig(i);
    //                    }
    //                }
    //                break;
    //            }
    //        case CanveType.Game: break;
    //    }
    //}
    #region �л�ui����
    [Button("�л�ui����")]
    public void SwitchCanves(CanveType canveType)
    {
        CurrentCanveType = canveType;
        roomCanvas.gameObject.SetActive(CurrentCanveType == CanveType.Room);
        configCanvas.gameObject.SetActive(CurrentCanveType == CanveType.Config);
        gameCanvas.gameObject.SetActive(CurrentCanveType == CanveType.Game);
        //�Բ�ͬ������г�ʼ��
        //����Ч��
        switch (CurrentCanveType)
        {
            case CanveType.Room: break;
            case CanveType.Config:
                {
                    InitConfigUI();
                    InitCharaList();
                    break;
                }
            case CanveType.Game: break;
        }

    }
    #endregion
    #region �������UI
    public void RefreshRoomList()
    {
        //��ѯ�����б�
    }
    public async void OpenCreatRoomUI()
    {
        creatRoomUI?.SetActive(true);
    }
    public async void CloseCreatRoomUI()
    {
        creatRoomUI?.SetActive(false);
    }
    public async void CreatRoom()
    {
        string roomID = await NetManager.CreatRoom(roomName.text, roomPassword.text);
        //��������
        JoinRoom(roomID);
    }
    public async void JoinRoom(string roomID)
    {
        //���뷿��
        bool isJoinSuccess = await NetManager.JoinRoom(roomID);
    }
    public async void LeaveRoom(string roomID)
    {
        //�뿪����
        bool isLeaveSuccess = await NetManager.LeaveRoom(roomID);
    }
    //���׼����
    bool IsBeReady { get; set; }
    public void PlayerBeReady()
    {
        if (!IsBeReady)
        {
            IsBeReady = true;
            //�������׼��
        }
        else
        {

        }
    }
    public void PlayerCancel()
    {

    }
    #endregion
    #region ���ý���UI
    [Header("У׼���������")]
    public GameObject IconItem;
    //��ʼ������ѡ���б�
    public void InitCharaList()
    {
        Chara[] charaEnum = (Chara[])(Enum.GetValues(typeof(Chara)));
        foreach (Chara chara in charaEnum)
        {
            var newItem = Instantiate(IconItem, IconItem.transform.parent);
            newItem.SetActive(true);
            Sprite icon = AssetBundleManager.Load<Sprite>("Icon", chara.ToString());
            newItem.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = icon;
            newItem.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Text>().text = chara.ToString();
            newItem.GetComponent<Button>().onClick.AddListener(() => SetCurrentChara(chara));
        }
    }
    public void OpenCharaSelectList()
    {
        charaSelectListUI.SetActive(true);
        //չ������
    }
    public void CloseCharaSelectList()
    {
        charaSelectListUI.SetActive(false);
        //���𶯻�
    }
    public void SetCurrentChara(Chara chara)
    {
        Debug.Log("��ť���" + chara.ToString());
        ConfigManager.Instance.SelectModel(chara);
        CloseCharaSelectList();
    }
    public void SetPlayerName(string name)
    {

       
        GameManager.SaveLocalUserData();
    }
    //��������ui����ѡ��

    bool IsFaceListOpenOnConfig;
    bool IsVoiceListOpenOnConfig;
    bool IsActionListOpenOnConfig;
    //��ʼ�����棬�ر�����
    public async void InitConfigUI()
    {
        await CloseCharaFaceListOnConfig();
        CloseCharaVoiceListOnConfig();
        CloseCharaActionList();
    }
    //չ����ɫ�����б�
    public async void OpenCharaFaceListOnConfig()
    {
        var IsFaceListOpen = IsFaceListOpenOnConfig;
        //�ص�����ѡ���Ӳ˵�
        InitConfigUI();
        //��������Ӳ˵�֮ǰδչ������չ��
        if (!IsFaceListOpen)
        {
            await CustomThread.TimerAsync(0.3f, progress =>
            {
                faceListOnConfig.GetComponent<RectTransform>().anchoredPosition = new(1400 + ((1 - progress) * 10), -600);
                faceListOnConfig.GetComponent<CanvasGroup>().alpha = progress;
            });
            IsFaceListOpenOnConfig = true;
        }
    }
    public async void SelectCharaFaceListOnConfig(int index)
    {
        //ѡ��ָ������
        GameManager.CurrentConfigChara.faceManager.SetFace(index);
        await CloseCharaFaceListOnConfig();
    }
    //�رս�ɫ�����б�
    public async Task CloseCharaFaceListOnConfig()
    {
        if (IsFaceListOpenOnConfig)
        {
            //�����ĸ���ť
            await CustomThread.TimerAsync(0.3f, progress =>
            {
                faceListOnConfig.GetComponent<RectTransform>().anchoredPosition = new Vector2(1400 + (progress * 10), -600);
                faceListOnConfig.GetComponent<CanvasGroup>().alpha = 1 - progress;
            });
            IsFaceListOpenOnConfig = false;
        }
    }
    //չ����ɫ�����б�
    [Button("չ����ɫ�����б�")]
    public async Task OpenCharaVoiceLisOntConfig()
    {
        var IsVoiceListOpen = IsVoiceListOpenOnConfig;
        //�ص�����ѡ���Ӳ˵�
        InitConfigUI();
        //��������Ӳ˵�֮ǰδչ������չ��
        if (!IsVoiceListOpen)
        {
            //����ָ����ɫ������
            string charaTag = GameManager.CurrentConfigChara.currentPlayerChara.ToString();
            var voices = AssetBundleManager
                .LoadAll<AudioClip>(charaTag, "Voice")
                .Where(voice => voice.name.StartsWith("V"))
                .ToList();
            Debug.Log($"����{GameManager.CurrentConfigChara.currentPlayerChara}������������Ϊ{voices.Count}");

            //���ɲ����̨������
            for (int i = voiceContentOnConfig.childCount; i < voices.Count; i++)
            {
                Instantiate(voiceItemOnConfiga, voiceContentOnConfig);
            }
            //��������̨��ѡ��
            foreach (Transform voiceItem in voiceContentOnConfig)
            {
                voiceItem.gameObject.SetActive(false);
            }
            //��ʼ��ָ������ѡ��
            for (int i = 0; i < voices.Count; i++)
            {
                Transform voiceItem = voiceContentOnConfig.GetChild(i);
                voiceItem.gameObject.SetActive(true);
                voiceItem.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1) + ":" + voices[i].name[2..];
                voiceItem.GetComponent<Button>().onClick.RemoveAllListeners();
                voiceItem.GetComponent<Button>().onClick.AddListener(() => SelectCharaVoiceListOnConfig(i));
            }

            //���ݼ��������ui�����ͺ�����
            await CustomThread.TimerAsync(0.3f, progress =>
            {
                faceListOnConfig.GetComponent<RectTransform>().anchoredPosition = new(-130 + ((1 - progress) * 10), 20);
                faceListOnConfig.GetComponent<CanvasGroup>().alpha = progress;
            });
            IsVoiceListOpenOnConfig = true;
        }
    }
    public async void SelectCharaVoiceListOnConfig(int id)
    {
        //ѡ��ָ������
        //GameManager.CurrentConfigChara.faceManager.SetVoice(audioClip);
        GameManager.CurrentConfigChara.faceManager.SetVoice(id);
        await CloseCharaVoiceListOnConfig();
    }
    //�رս�ɫ�����б�
    public Task CloseCharaVoiceListOnConfig()
    {
        return null;
    }
    //չ����ɫ�����б�
    public void OpenCharaActionList()
    {

    }
    //�رս�ɫ�����б�
    public void CloseCharaActionList()
    {

    }
    public void SetMicrophoneVolume(float value)
    {
        microphoneImage.GetComponent<RectTransform>().transform.position = new Vector3(0, -10, 0);
    }
    #endregion
    #region ��Ϸ����UI
    //��ʷ������Ϣ����
    public void RefreshPlayCardHistory(int count)
    {

    }
    //����
    bool isTimerStop = false;
    [Button("������ʱ��")]
    public async void StartTimer(float sec = 30)
    {
        StopTimer();
        isTimerStop = false;
        timerUI.SetActive(true);
        while (true)
        {
            timerUI.GetComponent<Image>().material.SetFloat("_Progress", (30 - sec) / 30f);
            timerUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ((int)sec).ToString();
            await Task.Delay(100);
            sec -= 0.1f;
            if (isTimerStop || sec <= 0)
            {
                break;
            }
        }
    }
    [Button("�رն�ʱ��")]
    public void StopTimer()
    {
        isTimerStop = true;
        timerUI.SetActive(false);
        timerUI.GetComponent<Image>().material.SetFloat("_Progress", 0);
    }
    //���ؼ�ʱ
    public void ShowPlayerOperation(float second)
    {
        StartTimer(second);
        playButtonUI.SetActive(true);
        questionButtonUI.SetActive(true);
    }
    //����������Ҳ���ui
    public void HidePlayerOperation()
    {
        StopTimer();
        playButtonUI?.SetActive(false);
        questionButtonUI?.SetActive(false);
    }
    public void PlayerCard()
    {
        if (!GameManager.currentClientPlayer.handCardManager.SelectCards.Any())
        {
            //������ʾ����ѡ��һ����
            Debug.Log("δѡ���ƣ��޷����");
        }
        else
        {
            Debug.Log("���ƴ�����޷�����");
            NetManager.PlayCard(GameManager.currentClientPlayer.handCardManager.SelectCards);
        }
    }
    public void QuestionCard()
    {
        if (GameManager.CardPlayHistory == 0)
        {
            //������ʾ����������
            Debug.Log("��ʷ�޿��ƴ�����޷�����");
        }
        else
        {
            NetManager.Question();
        }
    }
    #endregion
}
