using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDB : MonoBehaviour
{
    private static CardDB instance;
    public static CardDB Instance{get=>instance;}
    public CardGame[] cardList;

    private void Awake() {
        if(instance)
            Debug.LogError("More than 1 CardDB");
        instance=this;
    }
}
