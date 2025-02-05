using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;
//玩家角色管理器
public class PlayerManager : MonoBehaviour
{
    ////////////////////////注视点位置/////////////////////////
    [Header("固定点位")]
    public GameObject focusPoint;
    //桌子上卡牌放置位置
    public GameObject leftTablePoint;
    public GameObject rightTablePoint;
    public GameObject head;
    //左手手牌跟随目标
    public GameObject leftHand;
    //右手出牌跟随点目标
    public GameObject rightHand;
    //右手抢放置位置
    //手牌管理器
    ////////////////////////模型/////////////////////////
    [Header("相关模型")]
    public GameObject gun;
    public GameObject handCardsPoint;
    //public GameObject chara;
    public List<SkinnedMeshRenderer> charaMesh;
    ////////////////////////动作/////////////////////////
    [Header("动作")]
    public ActionType Idle;
    public ActionType HoldCardIdle;
    public ActionType ReloadBullets;
    public ActionType PickupCard;
    public ActionType PlayCard;
    public ActionType QuestionCard;
    public ActionType RaiseGun;
    public ActionType Shoot;
    public ActionType DropGun;
    public ActionType KnockedOut;
    public ActionType FinishPlayIdle;
    public ActionType Victory;
    ////////////////////////组件/////////////////////////
    [Header("角色状态")]
    //是否处于等待玩家操作阶段
    public bool IsWaitPlayerOperation;
    public Chara currentPlayerChara;
    public PlayerState currentPlayerState;
    public CardPosManager handCardManager => GetComponent<CardPosManager>();
    public FaceManager faceManager => GetComponent<FaceManager>();
    public Animator animator => transform.GetChild(0).GetComponent<Animator>();
    public RuntimeAnimatorController animatorController;
    //角色初始化
    [Button("初始化")]
    public void Init()
    {
        handCardsPoint.transform.position = leftTablePoint.transform.position;
        handCardsPoint.transform.eulerAngles = leftTablePoint.transform.eulerAngles;
        //gun
        //动作
    }
    public void WaitPlayerOperation(float second)
    {
        IsWaitPlayerOperation = true;
        handCardManager.IsWaitForPlayCard = true;
        //开启操作ui
        UIManager.Instance.ShowPlayerOperation(second);
    }
    public void StopPlayerOperation()
    {
        IsWaitPlayerOperation = false;
        handCardManager.IsWaitForPlayCard = false;
        //关闭操作ui
        UIManager.Instance.HidePlayerOperation();
    }
    [Button("设置人物动作")]
    public async Task SetActionAsync(int index)
    {
        var targetClip = index switch
        {
            0 => AnimationManager.GetAnimationClip(Idle),
            1 => AnimationManager.GetAnimationClip(HoldCardIdle),
            2 => AnimationManager.GetAnimationClip(ReloadBullets),
            3 => AnimationManager.GetAnimationClip(PickupCard),
            4 => AnimationManager.GetAnimationClip(PlayCard),
            5 => AnimationManager.GetAnimationClip(QuestionCard),
            6 => AnimationManager.GetAnimationClip(RaiseGun),
            7 => AnimationManager.GetAnimationClip(Shoot),
            8 => AnimationManager.GetAnimationClip(DropGun),
            9 => AnimationManager.GetAnimationClip(KnockedOut),
            10 => AnimationManager.GetAnimationClip(FinishPlayIdle),
            11 => AnimationManager.GetAnimationClip(Victory),
            _ => throw new System.NotImplementedException($"Index {index} is not implemented."),
        };
        
        // 播放动画
        animator.CrossFade(targetClip.name,0.1f);
        await Task.Delay((int)(targetClip.length * 1000));
        Debug.Log("播放完成");
    }
    [Header("校准")]
    
    public Vector3 LeftHandPointPos;
    public Vector3 LeftHandPointEula;
    [Button("校准")]


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
