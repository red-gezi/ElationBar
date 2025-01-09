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
    //[Header("登录")]
    //public TMP_InputField playerName;
    [Header("房间界面")]
    public Canvas roomCanvas;
    public Text roomName;
    public Text roomPassword;

    public GameObject creatRoomUI;
    [Header("配置界面")]
    public Canvas configCanvas;

    public GameObject charaSelectListUI;
    public Text playerName;

    [Header("游戏界面")]
    public Canvas gameCanvas;

    public GameObject timerUI;
    public GameObject playButtonUI;
    public GameObject questionButtonUI;

    [Header("音量")]
    public Image microphoneImage;
    private void Start()
    {

    }
    //public async void Login()
    //{
    //    string playerName = this.playerName.text;
    //    if (playerName == "")
    //    {
    //        playerName = "无名客";
    //    }
    //    if (await NetCommand.LoginAsync(playerName))
    //    {
    //        SwitchCanves(1);
    //    }
    //}
    #region 切换ui界面
    [Button("切换ui界面")]
    public void SwitchCanves(int index)
    {
        roomCanvas.gameObject.SetActive(index == 0);
        configCanvas.gameObject.SetActive(index == 1);
        gameCanvas.gameObject.SetActive(index == 2);
        //对不同界面进行初始化
        //动画效果
        switch (index)
        {
            case 0: break;
            case 1: break;
            case 2: break;
        }

    }
    #endregion
    #region 房间界面UI
    public void RefreshRoomList()
    {
        //查询房间列表
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
        //创建房间
        JoinRoom(roomID);
    }
    public async void JoinRoom(string roomID)
    {
        //加入房间
        bool isJoinSuccess = await NetCommand.JoinRoom(roomID);
    }
    public async void LeaveRoom(string roomID)
    {
        //离开房间
        bool isLeaveSuccess = await NetCommand.LeaveRoom(roomID);
    }
    //玩家准备好
    bool IsBeReady { get; set; }
    public void PlayerBeReady()
    {
        if (!IsBeReady)
        {
            IsBeReady = true;
            //发送玩家准备
        }
        else
        {

        }
    }
    public void PlayerCancel()
    {

    }
    #endregion
    #region 配置界面UI
    [Header("校准房玩家配置")]
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
        //展开动画
    }
    public void CloseCharaSelectList()
    {
        charaSelectListUI.SetActive(false);
        //收起动画
    }
    public void SetCurrentChara(Chara chara)
    {
        Debug.Log("按钮点击" + chara.ToString());
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
    //调整音量ui语音选项

    bool IsFaceListOpen;
    bool IsVoiceListOpen;
    bool IsActionListOpen;
    //初始化界面，关闭所有
    public void Init()
    {
        CloseCharaFaceList();
        CloseCharaVoiceList();
        CloseCharaActionList();
    }
    //展开角色表情列表
    public void OpenCharaFaceList()
    {
        //放大显示四个按钮
        Init();
        //首次点击展开表情列表
        if (!IsFaceListOpen)
        {

        }

    }
    public void SelectCharaFaceList(int index)
    {
        //选择指定表情
        GameManager.CurrentConfigChara.faceManager.SetFace(index);
        CloseCharaFaceList();
    }
    //关闭角色表情列表
    public void CloseCharaFaceList()
    {
        if (IsFaceListOpen)
        {
            //隐藏四个按钮

        }
    }
    //展开角色语音列表
    public void OpenCharaVoiceList()
    {
        //加载指定角色的语音
        string charaTag = GameManager.CurrentConfigChara.currentPlayerChara.ToString();
        var voices = AssetBundleManager
            .LoadAll<AudioClip>(charaTag, "Voice")
            .Where(voice => voice.name.StartsWith("V"))
            .ToList();
        //根据加载项调整ui数量和横坐标



    }
    //关闭角色语音列表
    public void CloseCharaVoiceList()
    {

    }
    //展开角色动作列表
    public void OpenCharaActionList()
    {

    }
    //关闭角色动作列表
    public void CloseCharaActionList()
    {

    }
    public void SetMicrophoneVolume(float value)
    {
        microphoneImage.GetComponent<RectTransform>().transform.position = new Vector3(0, -10, 0);
    }
    #endregion

    #region 游戏信息界面UI
    //历史出牌信息更新
    public void RefreshPlayCardHistory(int count)
    {

    }
    //静音
    bool isTimerStop = false;
    [Button("启动定时器")]
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
    [Button("关闭定时器")]
    public void StopTimer()
    {
        isTimerStop = true;
        timerUI.SetActive(false);
        timerUI.GetComponent<Image>().material.SetFloat("_Progress", 0);
    }
    #endregion
    #region 游戏交互UI
    //开关计时
    public void ShowPlayerOperation()
    {
        StartTimer();
        playButtonUI.SetActive(true);
        questionButtonUI.SetActive(true);
    }
    //隐藏所有玩家操作ui
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
            //弹窗提示至少选择一张牌
            Debug.Log("未选择卡牌，无法打出");
        }
        else
        {
            Debug.Log("卡牌打出，无法质疑");
            NetCommand.PlayCard(GameManager.currentClientPlayer.handCardManager.SelectCards);
        }
    }
    public void QuestionCard()
    {
        if (GameManager.CardPlayHistory == 0)
        {
            //弹窗提示必须打出过牌
            Debug.Log("历史无卡牌打出，无法质疑");
        }
        else
        {
            NetCommand.Question();
        }
    }
    #endregion
}
