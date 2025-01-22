using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CardDeckManager : GeziBehaviour<CardDeckManager>
{
    public GameObject cardModel;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            var newCard = Instantiate(cardModel, cardModel.transform.parent);
            newCard.transform.position += new Vector3(0, 0.002f, 0) * i;
        }
    }
    public async Task ShowTargetCard(CardType cardType)
    {
        var showCard = Instantiate(cardModel, cardModel.transform.parent);
        showCard.GetComponent<Card>().Init(cardType);
        await CustomThread.TimerAsync(0.5f, progress =>
        {
            showCard.transform.localPosition = Vector3.forward * 0.5f * progress;
            showCard.transform.localEulerAngles = Vector3.right * 90 * progress;
        });
        await CustomThread.TimerAsync(3f, async progress =>
        {
            showCard.transform.localEulerAngles = new Vector3(90, 0, 0);
            showCard.transform.Rotate(Vector3.up, 720f * progress, Space.World);
        });
        await CustomThread.TimerAsync(0.5f, progress =>
        {
            showCard.transform.localPosition = Vector3.forward * 0.5f * (1 - progress);
            showCard.transform.localEulerAngles = Vector3.right * 90 * (1 - progress);
        });
        Destroy(showCard);
    }

    internal List<Card> Draw5Cards(List<CardType> cardTypeList)
    {
        List<Card> newCards = new List<Card>();
        for (int i = 0; i < 5; i++)
        {
            var newCard = Instantiate(cardModel, cardModel.transform.parent);
            Card card = newCard.GetComponent<Card>();
            card.Init(cardTypeList[i]);
            newCards.Add(card);
        }
        return newCards;
    }
}
