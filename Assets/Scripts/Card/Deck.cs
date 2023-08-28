using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public CardGame[] deck;
    public CardGame container;
    public int x;
    public int deckSize;

    // Start is called before the first frame update
    void Start()
    {
        x=0;
        deckSize=40;
        for (int i = 0; i < deckSize; i++)
        {
            x=Random.Range(1,5);
            deck[i]=CardDB.Instance.cardList[x];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(deckSize<30)
            transform.GetChild(0).gameObject.SetActive(false);
        if(deckSize<20)
            transform.GetChild(1).gameObject.SetActive(false);
        if(deckSize<2)
            transform.GetChild(2).gameObject.SetActive(false);
        if(deckSize<1)
            transform.GetChild(3).gameObject.SetActive(false);
    }
    public void Shuffle()
    {
        for (int i = 0; i < deckSize; i++)
        {
            container=deck[i];
            int randomIndex=Random.Range(i,deckSize);
            deck[i]=deck[randomIndex];
            deck[randomIndex]=container;
        }
    }
}
