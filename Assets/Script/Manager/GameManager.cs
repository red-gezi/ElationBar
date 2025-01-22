using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public class GameManager : GeziBehaviour<GameManager>
{
    [Header("当前启动模式")]
    public PlayerMode CurrentPlayMode;
    public Camera gameCamera;
    //所有角色模型的根层级
    public GameObject charaRoot;
    public GameObject playerGroup;

    public bool IsConfigMode { get; set; } = !true;
    public static PlayerManager CurrentConfigChara { get; set; } = null;
    public List<Transform> playerChair;
    public static List<PlayerManager> gameCharas = new();
    //public HandCardManager handCardManager;
    public Dictionary<string, GameObject> charaModels = new();
    //出牌历史记录
    public static int CardPlayHistory { get; set; } = 0;
    //当前客户端玩家座位id
    public static int ClientChairID { get; set; }
    public static CardPosManager currentPlayerHandCardManager => gameCharas[ClientChairID].handCardManager;
    public static PlayerManager currentClientPlayer => gameCharas[ClientChairID];

    public static bool IsClientPlayer(int playerID) => playerID == ClientChairID;
    public int lastLosePlayerIndex = -1;
    public static string RoomID { get; set; }
    public static string PlayerName { get; set; }
    public static Chara PlayerChara { get; set; } 
    public static List<string> PlayerLocalData { get; set; }

    //当前执行选择操作的玩家索引;
    int currentPlayerIndex;
    public int CurrentPlayerIndex
    {
        get => currentPlayerIndex;
        set => currentPlayerIndex = value % gameCharas.Count;
    }
    static bool IsPlayCards { get; set; } = false;
    static bool IsQuestion { get; set; } = false;
    static bool IsQuit { get; set; } = false;
    List<int> SelectCardIndexs { get; set; }

    List<CardType> LastPlayCards = new();
    // Start is called before the first frame update
    async void Start()
    {
        await AssetBundleManager.Init("1", false);
        //初始化网络
        await NetManager.Init();
        //检查热更新
        //登录
        //初始化配置
        Init();
        switch (CurrentPlayMode)
        {
            case PlayerMode.Normal:
                UIManager.Instance.SwitchCanves( CanveType.Room);
                await NetManager.LoginAsync("shajin");

                break;
            case PlayerMode.TestConfig:
                
                UIManager.Instance.SwitchCanves(CanveType.Config);
                ConfigManager.Instance.SelectModel(Chara.李素裳);

                break;
            case PlayerMode.TestGameLogic:
                UIManager.Instance.SwitchCanves(CanveType.Game);
                await NetManager.GameStartMockAsync();
                break;
            default:
                break;
        }
    }
    private void Update()
    {

    }
    public void Init()
    {
        gameCharas.Clear();
        CardPlayHistory = 0;
        LoadLocalUserData();
        foreach (Transform chara in charaRoot.transform)
        {
            charaModels[chara.name] = chara.gameObject;
        }
    }
    private void OnApplicationQuit() => IsQuit = true;
    public static void LoadLocalUserData()
    {
        if (!File.Exists("UserData.ini"))
        {
            File.WriteAllLines("UserData.ini", new string[] {"0","测试者",((int)Chara.砂金).ToString()});
        }
        PlayerLocalData = File.ReadAllLines("UserData.ini").ToList();
    }
    public static void SaveLocalUserData()
    {
        //File.WriteAllLines("UserData.ini", PlayerLocalData);
    }
    //#region 客户端向服务端发出指令
    ///////////////////////////////客户端向服务端发出指令/////////////////////////////////////////////////
    //public void PlayerPlayCard()
    //{
    //    if (IsClientPlayer(currentPlayerIndex))
    //    {
    //        SelectCardIndexs = gameCharas[chairID].handCardManager.SelectCardIndexs;

    //        if (SelectCardIndexs.Any())
    //        {
    //            //通知服务器打出的牌
    //            IsPlayCards = true;
    //        }
    //    }

    //}
    //public void PlayerQuestionCard()
    //{
    //    //通知服务器质疑打出的牌
    //}
    //#endregion
    #region 客户端对服务端的指令响应
    /////////////////////////////客户端对服务端的指令响应/////////////////////////////////////////////////
    //加入房间
    internal static void NotifyJoinRoom(Room room)
    {
        Debug.Log(room.ToJson());
    }
    internal static async void NotifyGameInit(int currentChairID, List<UserInfo> playerInfos)
    {
        Debug.Log($"游戏初始化,接收到{playerInfos.Count}个玩家信息，当前玩家编号为{currentChairID}");

        ClientChairID = currentChairID;
        //生成四个角色
        for (int i = 0; i < playerInfos.Count; i++)
        {
            Instance.charaModels.TryGetValue(playerInfos[i].PlayerRole.ToString(), out GameObject model);
            GameObject newChara = Instantiate(model, Instance.playerChair[i].transform);
            newChara.SetActive(true);
            newChara.transform.localPosition = Vector3.up;
            newChara.transform.localEulerAngles = new Vector3(0, 0, 0);
            PlayerManager newPlayer = newChara.GetComponent<PlayerManager>();
            newPlayer.Init();
            gameCharas.Add(newPlayer);
            //var player = players[chairID];
            //player.focusPoint.transform.position = new Vector3(
            //    player.focusPoint.transform.position.x,
            //    player.head.transform.position.y,
            //    player.focusPoint.transform.position.z);
        }
        //给玩家角色加装摄像头
        await Task.Delay(5000);
        CameraManager.Instance.target = gameCharas[ClientChairID].focusPoint;
        Instance.gameCamera.transform.position = gameCharas[ClientChairID].head.transform.position;
        Instance.gameCamera.transform.eulerAngles = gameCharas[ClientChairID].head.transform.eulerAngles;
    }
    internal static void NotifyTurnInit()
    {
        //清空历史记录ui，提示新一局开始
    }
    internal static async void NotifyShowTargetCard(CardType cardType)
    {
        Debug.Log("展示当局卡牌" + cardType.ToString());
        //生成1张牌,旋转展示
        await CardDeckManager.Instance.ShowTargetCard(cardType);
    }

    internal static void NotifyLoadBullet()
    {
        Debug.Log("换子弹动画");
    }

    internal static async void NotifyDraw5Cards(List<CardType> cardsType)
    {
        Debug.Log("角色抽5张卡");

        for (int i = 0; i < gameCharas.Count; i++)
        {
            gameCharas[i].handCardManager.angel = 15;
            //如果是玩家本人抽牌，则为通知的牌，不然是空牌
            //每个牌堆生成5张牌
            var newCards = CardDeckManager.Instance.Draw5Cards(IsClientPlayer(i) ? cardsType : new() { CardType.N, CardType.N, CardType.N, CardType.N, CardType.N });
            for (int j = 0; j < newCards.Count; j++)
            {
                gameCharas[i].handCardManager.DrawCard(newCards[j]);
                await Task.Delay(100);
            }
            //移动至玩家手上，展开
            await Task.Delay(500);
            gameCharas[i].handCardManager.isControlCard = true;
        }
    }
    internal static async void NotifyWaitForPlayer(int currentPlayerIndex, float second)
    {
        Debug.Log($"等待玩家{currentPlayerIndex}操作");
        //转盘旋转
        //如果对象是本客户端玩家，开启倒计时，开启操作选项，开启卡牌焦点
        if (IsClientPlayer(currentPlayerIndex))
        {
            gameCharas[currentPlayerIndex].WaitPlayerOperation(second);
        }
    }
    internal static void NotifyPlayCard(int currentPlayerIndex, List<int> selectCardIndexs)
    {
        Debug.Log($"玩家{currentPlayerIndex}出牌{selectCardIndexs.ToJson()}");
        CardPlayHistory = selectCardIndexs.Count;
        UIManager.Instance.RefreshPlayCardHistory(selectCardIndexs.Count);
        //客户端播放卡牌打出动画
        gameCharas[currentPlayerIndex].handCardManager.PlayCard(selectCardIndexs);
        //如果对象是本客户端玩家，开启倒计时，开启操作选项，开启卡牌焦点
        if (IsClientPlayer(currentPlayerIndex))
        {
            gameCharas[currentPlayerIndex].StopPlayerOperation();
        }
    }

    internal static void NotifyQuestion(int currentPlayerIndex)
    {
        //玩家质疑
        Debug.Log($"玩家{currentPlayerIndex}质疑");
        //如果对象是本客户端玩家，开启倒计时，开启操作选项，开启卡牌焦点
        if (IsClientPlayer(currentPlayerIndex))
        {
            gameCharas[currentPlayerIndex].StopPlayerOperation();
        }

    }

    internal static void NotifyPlayerWin(int currentPlayerIndex)
    {
        //切换摄像机视角，摆胜利pose
        Debug.Log($"玩家{currentPlayerIndex}胜利");
    }

    internal static void NotifyPlayerShot(int currentPlayerIndex, bool isSurvival)
    {
        Debug.Log($"玩家{currentPlayerIndex}举枪");

        //客户端播放玩家射击自己
        //举枪动画
        //等待5秒
        //开枪
        if (isSurvival)
        {
            Debug.Log($"玩家{currentPlayerIndex}存活");

        }
        else
        {
            Debug.Log($"玩家{currentPlayerIndex}死亡");
            //播放打死
            //如果是玩家，屏幕特效
            if (IsClientPlayer(currentPlayerIndex))
            {

            }
        }
    }

    internal static void NotifyPlayerAgain()
    {
        //询问玩家是否开始下一局匹配
    }

    
    #endregion
}
