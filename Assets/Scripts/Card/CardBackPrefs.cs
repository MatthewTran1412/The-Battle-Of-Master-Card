using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBackPrefs : MonoBehaviour
{
    public GameObject Deck;
    public GameObject It;

    // Update is called once per frame
    private void Start() {
        Deck=GameObject.Find("Deck");   
    }
    void Update()
    {
        It.transform.SetParent(Deck.transform);
        It.transform.localScale = Vector3.one;
        It.transform.position= new Vector3(transform.position.x,transform.position.y,-48);
        It.transform.eulerAngles = new Vector3(25,0,0);
    }
}
