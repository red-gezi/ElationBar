using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : GeziBehaviour<UIManager>
{
    //[Header("登录")]
    //public TMP_InputField playerName;

    public CanveType CurrentCanveType { get; set; }
    [Header("房间界面")]
    public Canvas roomCanvas;
    public Text roomName;
    public Text roomPassword;

    public GameObject creatRoomUI;
    [Header("配置界面")]
    public Canvas configCanvas;

    public GameObject charaSelectListUI;
    public Text playerName;
    public Transform actionListOnConfig;
    public Transform faceListOnConfig;
    public Transform voiceListOnConfig;
    public Transform voiceContentOnConfig;
    public Transform voiceItemOnConfiga;
    [Header("游戏界面")]
    public Canvas gameCanvas;
    public Transform faceListOnGame;
    public Transform voiceListOnGame;
    public GameObject timerUI;
    public GameObject playButtonUI;
    public GameObject questionButtonUI;

    [Header("音量")]
    public Image microphoneImage;
    #region 切换ui界面
    [Button("切换ui界面")]
    public void SwitchCanves(CanveType canveType)
    {
        CurrentCanveType = canveType;
        roomCanvas.gameObject.SetActive(CurrentCanveType == CanveType.Room);
        configCanvas.gameObject.SetActive(CurrentCanveType == CanveType.Config);
        gameCanvas.gameObject.SetActive(CurrentCanveType == CanveType.Game);
        //对不同界面进行初始化
        //动画效果
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
        string roomID = await NetManager.CreatRoom(roomName.text, roomPassword.text);
        //创建房间
        JoinRoom(roomID);
    }
    public async void JoinRoom(string roomID)
    {
        //加入房间
        bool isJoinSuccess = await NetManager.JoinRoom(roomID);
    }
    public async void LeaveRoom(string roomID)
    {
        //离开房间
        bool isLeaveSuccess = await NetManager.LeaveRoom(roomID);
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
    //初始化人物选择列表
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
        CloseCharaSelectList();
    }
    public void SetPlayerName(string name)
    {


        GameManager.SaveLocalUserData();
    }
    //调整音量ui语音选项

    //bool IsFaceListOpenOnConfig;
    bool IsVoiceListOpenOnConfig;
    bool IsActionListOpenOnConfig;
    //初始化界面，关闭所有
    public async void InitConfigUI()
    {
        await CloseCharaActionListOnConfig();
        await CloseCharaFaceListOnConfig();
        await CloseCharaVoiceListOnConfig();
    }
    //展开角色动作列表
    public async void OpenCharaActionListOnConfig()
    {
        var IsActionListOpen = actionListOnConfig.gameObject.activeSelf;
        //关掉其他选项子菜单
        InitConfigUI();
        //如果表情子菜单之前未展开，则展开
        if (!IsActionListOpen)
        {
            actionListOnConfig.gameObject.SetActive(true);
            await CustomThread.TimerAsync(0.3f, progress =>
            {
                actionListOnConfig.GetComponent<RectTransform>().anchoredPosition = new(1400 + ((1 - progress) * 10), -600);
                actionListOnConfig.GetComponent<CanvasGroup>().alpha = progress;
            });
            //IsFaceListOpenOnConfig = true;
        }
    }
    public async void SelectCharaActionListOnConfig(int index)
    {
        //选择指定表情
        _ = GameManager.CurrentConfigChara.SetActionAsync(index);
        await CloseCharaActionListOnConfig();
    }
    //关闭角色表情列表
    public async Task CloseCharaActionListOnConfig()
    {
        //if (IsFaceListOpenOnConfig)
        if (actionListOnConfig.gameObject.activeSelf)
        {
            //隐藏四个按钮
            await CustomThread.TimerAsync(0.3f, progress =>
            {
                actionListOnConfig.GetComponent<RectTransform>().anchoredPosition = new Vector2(1400 + (progress * 10), -600);
                actionListOnConfig.GetComponent<CanvasGroup>().alpha = 1 - progress;
            });
            //IsFaceListOpenOnConfig = false;
            actionListOnConfig.gameObject.SetActive(false);
        }
    }
    //展开角色表情列表
    public async void OpenCharaFaceListOnConfig()
    {
        //var IsFaceListOpen = IsFaceListOpenOnConfig;
        var IsFaceListOpen = faceListOnConfig.gameObject.activeSelf;
        //关掉其他选项子菜单
        InitConfigUI();
        //如果表情子菜单之前未展开，则展开
        if (!IsFaceListOpen)
        {
            faceListOnConfig.gameObject.SetActive(true);
            await CustomThread.TimerAsync(0.3f, progress =>
            {
                faceListOnConfig.GetComponent<RectTransform>().anchoredPosition = new(1400 + ((1 - progress) * 10), -600);
                faceListOnConfig.GetComponent<CanvasGroup>().alpha = progress;
            });
            //IsFaceListOpenOnConfig = true;
        }
    }
    public async void SelectCharaFaceListOnConfig(int index)
    {
        //选择指定表情
        GameManager.CurrentConfigChara.faceManager.SetFace(index);
        await CloseCharaFaceListOnConfig();
    }
    //关闭角色表情列表
    public async Task CloseCharaFaceListOnConfig()
    {
        //if (IsFaceListOpenOnConfig)
        if (faceListOnConfig.gameObject.activeSelf)
        {
            //隐藏四个按钮
            await CustomThread.TimerAsync(0.3f, progress =>
            {
                faceListOnConfig.GetComponent<RectTransform>().anchoredPosition = new Vector2(1400 + (progress * 10), -600);
                faceListOnConfig.GetComponent<CanvasGroup>().alpha = 1 - progress;
            });
            //IsFaceListOpenOnConfig = false;
            faceListOnConfig.gameObject.SetActive(false);
        }
    }
    //展开角色语音列表
    [Button("展开角色语音列表")]
    public async void OpenCharaVoiceLisOntConfig()
    {
        var IsVoiceListOpen = IsVoiceListOpenOnConfig;
        //关掉其他选项子菜单
        InitConfigUI();
        //如果表情子菜单之前未展开，则展开
        if (!IsVoiceListOpen)
        {
            voiceListOnConfig.gameObject.SetActive(true);
            //加载指定角色的语音
            string charaTag = GameManager.CurrentConfigChara.currentPlayerChara.ToString();
            var voices = AssetBundleManager
                .LoadAll<AudioClip>(charaTag, "Voice")
                .Where(voice => voice.name.StartsWith("V"))
                .ToList();
            Debug.Log($"加载{GameManager.CurrentConfigChara.currentPlayerChara}的语音，数量为{voices.Count}");

            //生成不足的台词数量
            for (int i = voiceContentOnConfig.childCount; i < voices.Count; i++)
            {
                Instantiate(voiceItemOnConfiga, voiceContentOnConfig);
            }
            //隐藏所有台词选项
            foreach (Transform voiceItem in voiceContentOnConfig)
            {
                voiceItem.gameObject.SetActive(false);
            }
            //初始化指定数量选项
            for (int i = 0; i < voices.Count; i++)
            {
                int rank = i;
                Transform voiceItem = voiceContentOnConfig.GetChild(i);
                voiceItem.gameObject.SetActive(true);
                voiceItem.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1) + ":" + voices[i].name[2..];
                voiceItem.GetComponent<Button>().onClick.RemoveAllListeners();
                voiceItem.GetComponent<Button>().onClick.AddListener(() => SelectCharaVoiceListOnConfig(rank));
                voiceItem.GetComponent<KeyBoardManager>().keyCode = KeyCode.Alpha1 + i;
            }

            //根据加载项调整ui数量和横坐标
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
        //选择指定语音
        GameManager.CurrentConfigChara.faceManager.SetVoice(id);
        await CloseCharaVoiceListOnConfig();
    }
    //关闭角色语音列表
    public Task CloseCharaVoiceListOnConfig()
    {
        if (IsVoiceListOpenOnConfig)
        {
            voiceListOnConfig.gameObject.SetActive(false);
            IsVoiceListOpenOnConfig = false;
        }
        return Task.Delay(1000);
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
    #region 游戏界面UI
    //历史出牌信息更新
    public void RefreshPlayCardHistory(int count)
    {

    }
    //静音
    bool isTimerStop = false;
    [Button("启动定时器")]
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
    [Button("关闭定时器")]
    public void StopTimer()
    {
        isTimerStop = true;
        timerUI.SetActive(false);
        timerUI.GetComponent<Image>().material.SetFloat("_Progress", 0);
    }
    //开关计时
    public void ShowPlayerOperation(float second)
    {
        StartTimer(second);
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
            NetManager.PlayCard(GameManager.currentClientPlayer.handCardManager.SelectCards);
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
            NetManager.Question();
        }
    }
    #endregion
}
