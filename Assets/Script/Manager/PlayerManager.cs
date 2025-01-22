using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//玩家角色管理器
public class PlayerManager : MonoBehaviour
{
    ////////////////////////注视点位置/////////////////////////
    public GameObject focusPoint;
    //桌子上卡牌放置位置
    public GameObject leftTablePoint;
    public GameObject rightTablePoint;
    ////////////////////////固定位置/////////////////////////
    public GameObject head;
    //左手手牌跟随目标
    public GameObject leftHand;
    //右手出牌跟随点目标
    public GameObject rightHand;
    //右手抢放置位置
    //手牌管理器
    ////////////////////////模型/////////////////////////
    public GameObject gun;
    public GameObject handCardsPoint;
    public GameObject chara;
    public List<SkinnedMeshRenderer> charaMesh;
    ////////////////////////组件/////////////////////////
    public CardPosManager handCardManager => GetComponent<CardPosManager>();
    public FaceManager faceManager => GetComponent<FaceManager>();
    //是否处于等待玩家操作阶段
    public bool IsWaitPlayerOperation;
    public Chara currentPlayerChara;
    public PlayerState currentPlayerState;
    //角色初始化
    public void Init()
    {
        handCardsPoint.transform.position = leftTablePoint.transform.position;
        handCardsPoint.transform.eulerAngles = leftTablePoint.transform.eulerAngles;
        //gun
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
