using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

public class CardPosManager : MonoBehaviour
{
    CharaManager playerManager;
    public List<Card> HandCards { get; set; } = new();
    public List<Card> SelectCards { get; set; } = new();
    public List<int> SelectCardIndexs => SelectCards.Select(card => HandCards.IndexOf(card)).ToList();
    public Transform handCardsPoint => playerManager.handCardsPoint;

    public Transform leftTablePoint => playerManager.leftTablePoint;
    public Transform leftHandPoint => playerManager.leftHandPoint;
    public Transform rightHandPoint => playerManager.rightHandPoint;
    public Transform cardDeckPoint;
    //判断是否已捡起
    public bool isPickUp;
    [HideInInspector]
    public Card focusCard;
    [HideInInspector]
    public int focusCardIndex = 0;
    [HideInInspector]
    public bool IsWaitForPlayCard;
    public float angel;
    //卡组展开值
    public float expandedValue;
    public Vector2 rotatePos;
    [HideInInspector]
    public bool isControlCard = false;


    // Update is called once per frame
    private void Awake()
    {
        playerManager = GetComponent<CharaManager>();
    }
    void Update()
    {
        //处于受控状态时，计算卡牌位置
        if (isControlCard)
        {
            RefreshCardPos();
        }
        RefreshHandCardPos();
        if (IsWaitForPlayCard)
        {
            //左右切换注视牌
            //空格选中
            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (SelectCards.Contains(focusCard))
                {
                    SelectCards.Remove(focusCard);
                }
                else
                {
                    SelectCards.Add(focusCard);
                    if (SelectCards.Count > 3)
                    {
                        SelectCards.RemoveAt(0);
                    }
                }
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                focusCardIndex = (focusCardIndex + 1) % HandCards.Count;
                focusCard = HandCards[focusCardIndex];
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                focusCardIndex = (focusCardIndex - 1 + HandCards.Count) % HandCards.Count;
                focusCard = HandCards[focusCardIndex];
            }
        }
    }
    public void PlayerControlOn()
    {
        focusCardIndex = 0;
        if (HandCards.Count > 0)
        {
            focusCard = HandCards[focusCardIndex];
            IsWaitForPlayCard = true;
        }
    }
    public void PlayerControlOff()
    {

        SelectCards = new List<Card>();
        focusCard = null;
        IsWaitForPlayCard = false;
    }
    //清空所有卡牌实例
    public async void InitCards()
    {

        isPickUp = false;
        CollapseDeck();
        for (int i = handCardsPoint.childCount - 1; i >= 0; i--)
        {
            HandCards.ForEach(card => Destroy(card.gameObject));
            HandCards.Clear();
            //Destroy(child.gameObject);
        }
    }
    internal async void DrawCards(List<CardType> cardsType)
    {
        Debug.Log("角色抽5张卡");
        var newCards = CardDeckManager.Instance.Draw5Cards(cardsType);
        for (int i = 0; i < newCards.Count; i++)
        {
            DrawCard(newCards[i]);
            await Task.Delay(100);
        }
        isControlCard = true;
        //gameCharas[i].cardPosManager.isControlCard = true;
        //移动至玩家手上，展开
        //await Task.Delay(500);
        //isControlCard = true;
    }
    public async void DrawCard(Card card)
    {
        //将卡牌移动到指定位置
        card.transform.parent = handCardsPoint;
        //加入手牌管理
        HandCards.Add(card.GetComponent<Card>());
        Vector3 startPoint = card.transform.localPosition;
        await CustomThread.TimerAsync(0.5f, progress =>
        {
            card.transform.localPosition = Vector3.Lerp(startPoint,new Vector3(rotatePos.x, rotatePos.y, 0),progress);
        });
    }
    public async void PickUpCards()
    {
        _ = CustomThread.DelayRun(2, () =>
        {
            isPickUp = true;
        });
        //设置卡牌延迟展开
        _ = CustomThread.DelayRun(4, () =>
        {
            ExpandDeck();
        });
    }

    public async void PlayCard(List<int> SelectCardIndexs)
    {
        //移除卡牌，刷新位置
        var cards = SelectCardIndexs.Select(i => HandCards[i]);
        cards.ForEach(card => HandCards.Remove(card));
        //卡牌加入到右手牌位置
        cards.ForEach(card => card.transform.parent = rightHandPoint);
        await CustomThread.TimerAsync(1, progress =>
        {
            cards.ForEach(card => card.transform.localPosition = Vector3.Lerp(card.transform.position, Vector3.zero, 0.1f));
        });
        //跟随动画
        await Task.Delay(1000);
        //卡牌移动到桌面随机位置
        await CustomThread.TimerAsync(1, progress =>
        {
            cards.ForEach(card => card.transform.localPosition = Vector3.Lerp(card.transform.position, cardDeckPoint.position, 0.1f));
        });
        await Task.Delay(1000);
        //卡牌移动至牌堆
        //cards.ForEach(card => Destroy(card.gameObject));
    }
    async void ExpandDeck()
    {
        await CustomThread.TimerAsync(1, progress =>
        {
            expandedValue = progress;
        });
    }

    async void CollapseDeck()
    {
        await CustomThread.TimerAsync(1, progress =>
        {
            expandedValue = 1 - progress;
        });
    }
    //刷新手牌点位
    public void RefreshHandCardPos()
    {
        Vector3 targetPosition = isPickUp ? leftHandPoint.localPosition : leftTablePoint.localPosition;
        Quaternion targetRotation = isPickUp ? leftHandPoint.rotation : leftTablePoint.rotation;
        float lerpSpeed = Time.deltaTime * 5.0f;
        handCardsPoint.localPosition = Vector3.Lerp(handCardsPoint.localPosition, targetPosition, lerpSpeed);
        //handCardsPoint.localRotation = Quaternion.Slerp(handCardsPoint.localRotation, targetRotation, lerpSpeed);
        handCardsPoint.rotation = Quaternion.Slerp(handCardsPoint.rotation, targetRotation, lerpSpeed);
        //    handCardsPoint.position = Vector3.Lerp(handCardsPoint.position, isPickUp?leftHandPoint.position:leftTablePoint.position,Time.deltaTime*5);
        //    handCardsPoint.eulerAngles = Vector3.Lerp(handCardsPoint.eulerAngles, isPickUp?leftHandPoint.eulerAngles : leftTablePoint.eulerAngles, Time.deltaTime*5);
    }
    //刷新卡牌位置
    public void RefreshCardPos()
    {
        int middleCount = HandCards.Count / 2;
        //移除卡牌，刷新位置
        for (int i = 0; i < HandCards.Count; i++)
        {
            var card = HandCards[i];
            card.transform.localPosition = new Vector3(rotatePos.x, rotatePos.y + (card.isSelect ? 0.3f : 0), i * 0.01f) * expandedValue;
            card.transform.localEulerAngles = Vector3.zero;
            card.transform.RotateAround(handCardsPoint.position, handCardsPoint.forward, angel * (i - middleCount) * expandedValue);
            card.RefreshState();
        }
    }
}
