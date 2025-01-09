using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : GeziBehaviour<UIManager>
{
    //[Header("��¼")]
    //public TMP_InputField playerName;
    [Header("�������")]
    public Canvas roomCanvas;
    public Text roomName;
    public Text roomPassword;

    public GameObject creatRoomUI;
    [Header("���ý���")]
    public Canvas configCanvas;

    public GameObject charaSelectListUI;
    public Text playerName;

    [Header("��Ϸ����")]
    public Canvas gameCanvas;

    public GameObject timerUI;
    public GameObject playButtonUI;
    public GameObject questionButtonUI;

    [Header("����")]
    public Image microphoneImage;
    private void Start()
    {

    }
    //public async void Login()
    //{
    //    string playerName = this.playerName.text;
    //    if (playerName == "")
    //    {
    //        playerName = "������";
    //    }
    //    if (await NetCommand.LoginAsync(playerName))
    //    {
    //        SwitchCanves(1);
    //    }
    //}
    #region �л�ui����
    [Button("�л�ui����")]
    public void SwitchCanves(int index)
    {
        roomCanvas.gameObject.SetActive(index == 0);
        configCanvas.gameObject.SetActive(index == 1);
        gameCanvas.gameObject.SetActive(index == 2);
        //�Բ�ͬ������г�ʼ��
        //����Ч��
        switch (index)
        {
            case 0: break;
            case 1: break;
            case 2: break;
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
        string roomID = await NetCommand.CreatRoom(roomName.text, roomPassword.text);
        //��������
        JoinRoom(roomID);
    }
    public async void JoinRoom(string roomID)
    {
        //���뷿��
        bool isJoinSuccess = await NetCommand.JoinRoom(roomID);
    }
    public async void LeaveRoom(string roomID)
    {
        //�뿪����
        bool isLeaveSuccess = await NetCommand.LeaveRoom(roomID);
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
        GameManager.PlayerChara = chara;
        GameManager.SaveLocalUserData();
        CloseCharaSelectList();
    }
    public void SetPlayerName(Chara chara)
    {

        GameManager.PlayerChara = chara;
        GameManager.SaveLocalUserData();
    }
    //��������ui����ѡ��

    bool IsFaceListOpen;
    bool IsVoiceListOpen;
    bool IsActionListOpen;
    //��ʼ�����棬�ر�����
    public void Init()
    {
        CloseCharaFaceList();
        CloseCharaVoiceList();
        CloseCharaActionList();
    }
    //չ����ɫ�����б�
    public void OpenCharaFaceList()
    {
        //�Ŵ���ʾ�ĸ���ť
        Init();
        //�״ε��չ�������б�
        if (!IsFaceListOpen)
        {

        }

    }
    public void SelectCharaFaceList(int index)
    {
        //ѡ��ָ������
        GameManager.CurrentConfigChara.faceManager.SetFace(index);
        CloseCharaFaceList();
    }
    //�رս�ɫ�����б�
    public void CloseCharaFaceList()
    {
        if (IsFaceListOpen)
        {
            //�����ĸ���ť

        }
    }
    //չ����ɫ�����б�
    public void OpenCharaVoiceList()
    {
        //����ָ����ɫ������
        string charaTag = GameManager.CurrentConfigChara.currentPlayerChara.ToString();
        var voices = AssetBundleManager
            .LoadAll<AudioClip>(charaTag, "Voice")
            .Where(voice => voice.name.StartsWith("V"))
            .ToList();
        //���ݼ��������ui�����ͺ�����



    }
    //�رս�ɫ�����б�
    public void CloseCharaVoiceList()
    {

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

    #region ��Ϸ��Ϣ����UI
    //��ʷ������Ϣ����
    public void RefreshPlayCardHistory(int count)
    {

    }
    //����
    bool isTimerStop = false;
    [Button("������ʱ��")]
    public async void StartTimer(int sec = 30)
    {
        StopTimer();
        isTimerStop = false;
        timerUI.SetActive(true);
        timerUI.GetComponent<Image>().material.SetFloat("_Progress", 0);
        timerUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = sec.ToString();
        while (true)
        {
            await Task.Delay(1000);
            sec--;
            timerUI.GetComponent<Image>().material.SetFloat("_Progress", (30 - sec) / 30f);
            timerUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = sec.ToString();
            if (isTimerStop|| sec == 0)
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
    #endregion
    #region ��Ϸ����UI
    //���ؼ�ʱ
    public void ShowPlayerOperation()
    {
        StartTimer();
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
            NetCommand.PlayCard(GameManager.currentClientPlayer.handCardManager.SelectCards);
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
            NetCommand.Question();
        }
    }
    #endregion
}
