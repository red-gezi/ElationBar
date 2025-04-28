using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
//玩家角色管理器
public class CharaManager : MonoBehaviour
{
    ////////////////////////注视点位置/////////////////////////
    [Header("固定点位")]
    public Transform focusPoint;
    //桌子上卡牌放置位置
    public Transform leftTablePoint;
    //桌子上枪放置位置
    public Transform rightTablePoint;
    public Transform head;
    //左手手牌跟随目标
    public Transform leftHandPoint;
    //右手出牌跟随点目标
    public Transform rightHandPoint;
    //右手抢放置位置
    //手牌管理器
    ////////////////////////模型/////////////////////////
    [Header("相关模型")]
    public Transform gun;
    public Transform handCardsPoint;
    //public GameObject chara;
    public List<SkinnedMeshRenderer> charaMesh;
    ////////////////////////动作/////////////////////////
    [Header("动作")]
    public ActionSubType Idle;
    public ActionSubType HoldCardIdle;
    public ActionSubType ReloadBullets;
    public ActionSubType PickupCard;
    public ActionSubType PlayCard;
    public ActionSubType QuestionCard;
    public ActionSubType RaiseGun;
    public ActionSubType Shoot;
    public ActionSubType DropGun;
    public ActionSubType KnockedOut;
    public ActionSubType FinishPlayIdle;
    public ActionSubType Victory;
    ////////////////////////组件/////////////////////////
    [Header("角色状态")]
    //是否处于等待玩家操作阶段
    public bool IsWaitPlayerOperation;
    public Chara currentPlayerChara;
    public PlayerState currentPlayerState;
    public CardPosManager cardPosManager => GetComponent<CardPosManager>();
    public FaceManager faceManager => GetComponent<FaceManager>();
    public Animator animator => transform.GetChild(0).GetComponent<Animator>();
    //角色初始化
    [Button("初始化")]
    public void Init()
    {
        handCardsPoint.transform.position = leftTablePoint.transform.position;
        handCardsPoint.transform.eulerAngles = leftTablePoint.transform.eulerAngles;
        focusPoint.transform.position = head.transform.position + head.transform.forward;
        cardPosManager.InitCards();
        cardPosManager.DrawCards(new() { CardType.N, CardType.N, CardType.N, CardType.N, CardType.N });
        SetAction(ActionType.Idle);
        //gun
        //动作
    }
    public void WaitPlayerOperation(float second)
    {
        IsWaitPlayerOperation = true;
        cardPosManager.IsWaitForPlayCard = true;
        //开启操作ui
        UIManager.Instance.ShowPlayerOperation(second);
    }
    public void StopPlayerOperation()
    {
        IsWaitPlayerOperation = false;
        cardPosManager.IsWaitForPlayCard = false;
        //关闭操作ui
        UIManager.Instance.HidePlayerOperation();
    }
    //通过ui设置动作
    [Button("设置人物动作")]
    public async Task SetActionAsync(int index)
    {
        var targetClip = index switch
        {
            0 => PlayCharaAction(Idle),
            1 => PlayCharaAction(HoldCardIdle),
            2 => PlayCharaAction(ReloadBullets),
            3 => PlayCharaAction(PickupCard),
            4 => PlayCharaAction(PlayCard),
            5 => PlayCharaAction(QuestionCard),
            6 => PlayCharaAction(RaiseGun),
            7 => PlayCharaAction(Shoot),
            8 => PlayCharaAction(DropGun),
            9 => PlayCharaAction(KnockedOut),
            10 => PlayCharaAction(FinishPlayIdle),
            11 => PlayCharaAction(Victory),
            _ => throw new System.NotImplementedException($"Index {index} is not implemented."),
        };
    }
    
    public async void SetAction(ActionType actionType)
    {
        Task targetClip = actionType switch
        {
            ActionType.Idle => PlayCharaAction(Idle),
            ActionType.HoldCardIdle => PlayCharaAction(HoldCardIdle),
            ActionType.ReloadBullets => PlayCharaAction(ReloadBullets),
            ActionType.PickupCard => PlayCharaAction(PickupCard),
            ActionType.PlayCard => PlayCharaAction(PlayCard),
            ActionType.QuestionCard => PlayCharaAction(QuestionCard),
            ActionType.RaiseGun => PlayCharaAction(RaiseGun),
            ActionType.Shoot => PlayCharaAction(Shoot),
            ActionType.DropGun => PlayCharaAction(DropGun),
            ActionType.KnockedOut => PlayCharaAction(KnockedOut),
            ActionType.FinishPlayIdle => PlayCharaAction(FinishPlayIdle),
            ActionType.Victory => PlayCharaAction(Victory),
            _ => throw new System.NotImplementedException($"ActionType {actionType} is not implemented."),
        };
    }
    public async Task PlayCharaAction(ActionSubType actionType)
    {
        //初始化牌组道具位置
        switch (actionType)
        {
            case ActionSubType.待机_男性:
                break;
            case ActionSubType.待机_女性:
            case ActionSubType.待机_花火:
                //设置卡牌位置
                //设置卡牌收拢
                break;
            case ActionSubType.取牌_男性:
                cardPosManager.PickUpCards();


                break;
            case ActionSubType.出牌_男性:
                cardPosManager.PlayCard(new List<int> { 0 });
                break;
            default:
                //设置卡牌位置

                break;
        }
        //初始化枪械道具位置
        switch (actionType)
        {
            case ActionSubType.开枪_男性:
            case ActionSubType.放下枪_男性:
            case ActionSubType.被打晕_男性:
                //设置手枪初始位置在手上
                break;
            default:
                //设置手枪初始位置在桌上
                break;
        }

        var targetClip = AnimationManager.GetAnimationClip(actionType);
        // 播放动画
        animator.CrossFade(targetClip.name, 0.1f);
        await Task.Delay((int)(targetClip.length * 1000));
        Debug.Log("播放完成");
    }

    private void Update()
    {
        //当前角色处于等待操作状态
        if (IsWaitPlayerOperation)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                UIManager.Instance.PlayerCard();
            }
            //质疑对方
            if (Input.GetKeyDown(KeyCode.C))
            {
                UIManager.Instance.QuestionCard();
            }
        }
    }
}
