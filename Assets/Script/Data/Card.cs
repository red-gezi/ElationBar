using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public List<Texture2D> cardTexture;
    GameObject model => gameObject;
    //当前位于手牌列表
    public CardType CurrentCardType {  get;  set; }
    //处于配置人物演示时无法注释和选择卡牌
    public bool isFocus => GameManager.IsConfigMode ? false : GameManager.currentClientPlayer.cardPosManager.focusCard == this;
    public bool isSelect =>GameManager.IsConfigMode?false: GameManager.currentClientPlayer.cardPosManager.SelectCards.Contains(this);
    public void RefreshState()
    {

        GetComponent<Renderer>().material.SetInt("_IsFocus", isFocus ? 1 : 0);
        GetComponent<Renderer>().material.SetInt("_IsSelect", isSelect ? 1 : 0);
       
    }
    public void Init(CardType cardType)
    {
        CurrentCardType = cardType;
        GetComponent<Renderer>().material.SetTexture("_MainTex", cardTexture[(int)CurrentCardType]);
    }
}
