using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CardData
{
    public int id;
    public string cardName;
    public int cost;
    public int power;
    public string cardDescription;
    public string color;

    public Sprite image;

    public CardData(){}
    public CardData(int Id, string CardName, int Cost, int Power, string CardDescription,Sprite Image,string Color)
    {
        id=Id;
        cardName=CardName;
        cost=Cost;
        power=Power;
        cardDescription=CardDescription;
        image=Image;
        color=Color;
    }
}
