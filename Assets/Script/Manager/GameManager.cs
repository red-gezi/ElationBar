using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public class GameManager : GeziBehaviour<GameManager>
{
    [Header("��ǰ����ģʽ")]
    public PlayerMode CurrentPlayMode;
    public Camera gameCamera;
    //���н�ɫģ�͵ĸ��㼶
    public GameObject charaRoot;
    public GameObject playerGroup;

    public bool IsConfigMode { get; set; } = !true;
    public static PlayerManager CurrentConfigChara { get; set; } = null;
    public List<Transform> playerChair;
    public static List<PlayerManager> gameCharas = new();
    //public HandCardManager handCardManager;
    public Dictionary<string, GameObject> charaModels = new();
    //������ʷ��¼
    public static int CardPlayHistory { get; set; } = 0;
    //��ǰ�ͻ��������λid
    public static int ClientChairID { get; set; }
    public static CardPosManager currentPlayerHandCardManager => gameCharas[ClientChairID].handCardManager;
    public static PlayerManager currentClientPlayer => gameCharas[ClientChairID];

    public static bool IsClientPlayer(int playerID) => playerID == ClientChairID;
    public int lastLosePlayerIndex = -1;
    public static string RoomID { get; set; }
    public static string PlayerName { get; set; }
    public static Chara PlayerChara { get; set; } 
    public static List<string> PlayerLocalData { get; set; }

    //��ǰִ��ѡ��������������;
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
        //��ʼ������
        await NetManager.Init();
        //����ȸ���
        //��¼
        //��ʼ������
        Init();
        switch (CurrentPlayMode)
        {
            case PlayerMode.Normal:
                UIManager.Instance.SwitchCanves( CanveType.Room);
                await NetManager.LoginAsync("shajin");

                break;
            case PlayerMode.TestConfig:
                
                UIManager.Instance.SwitchCanves(CanveType.Config);
                ConfigManager.Instance.SelectModel(Chara.������);

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
            File.WriteAllLines("UserData.ini", new string[] {"0","������",((int)Chara.ɰ��).ToString()});
        }
        PlayerLocalData = File.ReadAllLines("UserData.ini").ToList();
    }
    public static void SaveLocalUserData()
    {
        //File.WriteAllLines("UserData.ini", PlayerLocalData);
    }
    //#region �ͻ��������˷���ָ��
    ///////////////////////////////�ͻ��������˷���ָ��/////////////////////////////////////////////////
    //public void PlayerPlayCard()
    //{
    //    if (IsClientPlayer(currentPlayerIndex))
    //    {
    //        SelectCardIndexs = gameCharas[chairID].handCardManager.SelectCardIndexs;

    //        if (SelectCardIndexs.Any())
    //        {
    //            //֪ͨ�������������
    //            IsPlayCards = true;
    //        }
    //    }

    //}
    //public void PlayerQuestionCard()
    //{
    //    //֪ͨ���������ɴ������
    //}
    //#endregion
    #region �ͻ��˶Է���˵�ָ����Ӧ
    /////////////////////////////�ͻ��˶Է���˵�ָ����Ӧ/////////////////////////////////////////////////
    //���뷿��
    internal static void NotifyJoinRoom(Room room)
    {
        Debug.Log(room.ToJson());
    }
    internal static async void NotifyGameInit(int currentChairID, List<UserInfo> playerInfos)
    {
        Debug.Log($"��Ϸ��ʼ��,���յ�{playerInfos.Count}�������Ϣ����ǰ��ұ��Ϊ{currentChairID}");

        ClientChairID = currentChairID;
        //�����ĸ���ɫ
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
        //����ҽ�ɫ��װ����ͷ
        await Task.Delay(5000);
        CameraManager.Instance.target = gameCharas[ClientChairID].focusPoint;
        Instance.gameCamera.transform.position = gameCharas[ClientChairID].head.transform.position;
        Instance.gameCamera.transform.eulerAngles = gameCharas[ClientChairID].head.transform.eulerAngles;
    }
    internal static void NotifyTurnInit()
    {
        //�����ʷ��¼ui����ʾ��һ�ֿ�ʼ
    }
    internal static async void NotifyShowTargetCard(CardType cardType)
    {
        Debug.Log("չʾ���ֿ���" + cardType.ToString());
        //����1����,��תչʾ
        await CardDeckManager.Instance.ShowTargetCard(cardType);
    }

    internal static void NotifyLoadBullet()
    {
        Debug.Log("���ӵ�����");
    }

    internal static async void NotifyDraw5Cards(List<CardType> cardsType)
    {
        Debug.Log("��ɫ��5�ſ�");

        for (int i = 0; i < gameCharas.Count; i++)
        {
            gameCharas[i].handCardManager.angel = 15;
            //�������ұ��˳��ƣ���Ϊ֪ͨ���ƣ���Ȼ�ǿ���
            //ÿ���ƶ�����5����
            var newCards = CardDeckManager.Instance.Draw5Cards(IsClientPlayer(i) ? cardsType : new() { CardType.N, CardType.N, CardType.N, CardType.N, CardType.N });
            for (int j = 0; j < newCards.Count; j++)
            {
                gameCharas[i].handCardManager.DrawCard(newCards[j]);
                await Task.Delay(100);
            }
            //�ƶ���������ϣ�չ��
            await Task.Delay(500);
            gameCharas[i].handCardManager.isControlCard = true;
        }
    }
    internal static async void NotifyWaitForPlayer(int currentPlayerIndex, float second)
    {
        Debug.Log($"�ȴ����{currentPlayerIndex}����");
        //ת����ת
        //��������Ǳ��ͻ�����ң���������ʱ����������ѡ��������ƽ���
        if (IsClientPlayer(currentPlayerIndex))
        {
            gameCharas[currentPlayerIndex].WaitPlayerOperation(second);
        }
    }
    internal static void NotifyPlayCard(int currentPlayerIndex, List<int> selectCardIndexs)
    {
        Debug.Log($"���{currentPlayerIndex}����{selectCardIndexs.ToJson()}");
        CardPlayHistory = selectCardIndexs.Count;
        UIManager.Instance.RefreshPlayCardHistory(selectCardIndexs.Count);
        //�ͻ��˲��ſ��ƴ������
        gameCharas[currentPlayerIndex].handCardManager.PlayCard(selectCardIndexs);
        //��������Ǳ��ͻ�����ң���������ʱ����������ѡ��������ƽ���
        if (IsClientPlayer(currentPlayerIndex))
        {
            gameCharas[currentPlayerIndex].StopPlayerOperation();
        }
    }

    internal static void NotifyQuestion(int currentPlayerIndex)
    {
        //�������
        Debug.Log($"���{currentPlayerIndex}����");
        //��������Ǳ��ͻ�����ң���������ʱ����������ѡ��������ƽ���
        if (IsClientPlayer(currentPlayerIndex))
        {
            gameCharas[currentPlayerIndex].StopPlayerOperation();
        }

    }

    internal static void NotifyPlayerWin(int currentPlayerIndex)
    {
        //�л�������ӽǣ���ʤ��pose
        Debug.Log($"���{currentPlayerIndex}ʤ��");
    }

    internal static void NotifyPlayerShot(int currentPlayerIndex, bool isSurvival)
    {
        Debug.Log($"���{currentPlayerIndex}��ǹ");

        //�ͻ��˲����������Լ�
        //��ǹ����
        //�ȴ�5��
        //��ǹ
        if (isSurvival)
        {
            Debug.Log($"���{currentPlayerIndex}���");

        }
        else
        {
            Debug.Log($"���{currentPlayerIndex}����");
            //���Ŵ���
            //�������ң���Ļ��Ч
            if (IsClientPlayer(currentPlayerIndex))
            {

            }
        }
    }

    internal static void NotifyPlayerAgain()
    {
        //ѯ������Ƿ�ʼ��һ��ƥ��
    }

    
    #endregion
}
