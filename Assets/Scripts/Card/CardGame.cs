using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardGame", menuName = "Yugioh Game/CardGame", order = 0)]
public class CardGame : ScriptableObject {
    public int id;
    public string cardName;
    public int cost;
    public int power;
    public string cardDescription;
    public string color;
    public string Type;

    public Sprite image;

}
