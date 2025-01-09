using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

public class HandCardManager : MonoBehaviour
{
    public List<Card> HandCards { get; set; } = new();
    public List<Card> SelectCards { get; set; } = new();
    public List<int> SelectCardIndexs => SelectCards.Select(card=>HandCards.IndexOf(card)).ToList();
    public Transform handPoint;
    public Card focusCard;
    public int focusCardIndex = 0;
    public bool IsWaitForPlayCard;
    public float angel;
    public Vector2 rotatePos;
    public bool isControlCard = false;

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (isControlCard)
        {
            RefreshCardPos();
        }
        if (IsWaitForPlayCard)
        {
            //�����л�ע����
            //�ո�ѡ��
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
    public async void DrawCard(Card card)
    {
        //�������ƶ���ָ��λ��

        card.transform.parent = handPoint;
        //�������ƹ���
        HandCards.Add(card.GetComponent<Card>());
        Vector3 startPoint = card.transform.localPosition;
        await CustomThread.TimerAsync(0.5f, progress =>
        {
            card.transform.localPosition = Vector3.Lerp
                (
                startPoint,
                new Vector3(rotatePos.x, rotatePos.y, 0),
                progress
                );
        });
    }
    public void PlayCard(List<int> SelectCardIndexs)
    {
        //�Ƴ����ƣ�ˢ��λ��
    }
    public void RefreshCardPos()
    {
        int middleCount = HandCards.Count / 2;
        //�Ƴ����ƣ�ˢ��λ��
        for (int i = 0; i < HandCards.Count; i++)
        {
            var card = HandCards[i];
            card.transform.localPosition = new Vector3(rotatePos.x, rotatePos.y + (card.isSelect ? 0.3f : 0), i * 0.01f);
            card.transform.localEulerAngles = Vector3.zero;
            card.transform.RotateAround(handPoint.position, handPoint.forward, angel * (i - middleCount));
            card.RefreshState();
        }
    }
}
