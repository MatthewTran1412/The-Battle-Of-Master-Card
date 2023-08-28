using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class CardDetail : MonoBehaviour
{
    public CardGame CardsDetails;
    public int Id;

    public int id;
    public string cardName;
    public int cost;
    public int power;
    public string cardDescription;
    public string Type;

    public Text nameText;
    public Text costText;
    public Text powerText;
    public Text descriptionText;

    public Image cardImage;
    public Image cardFrame;

    public GameObject m_CardBack;
    public GameObject c_Card;
    public GameObject Card;

    [SerializeField]private bool isCardBack;
    // Start is called before the first frame update
    void Start()
    {
        CardsDetails= CardDB.Instance.cardList[Id];
        transform.localScale=Vector3.one;
        CardSetup();
    }

    // Update is called once per frame
    void Update()
    {
        PowerControl();
        CheckBackCard();
    }
    private void PowerControl()
    {
        if(power<=0)
            power=0;
        powerText.text=power.ToString();
    }
    private void CardSetup()
    {
        id=CardsDetails.id;
        cardName=CardsDetails.cardName;
        cost=CardsDetails.cost;
        power=CardsDetails.power;
        cardDescription=CardsDetails.cardDescription;
        Type=CardsDetails.Type;

        nameText.text=cardName;
        costText.text=cost.ToString();
        powerText.text=power.ToString();
        descriptionText.text=cardDescription;

        cardImage.sprite=CardsDetails.image;
        switch (CardsDetails.color)
        {
            case "Red":
                cardFrame.color=new Color(255,0,0);
                break;
            case "Green":
                cardFrame.color=new Color(0,255,0);
                break;
            case "Brown":
                cardFrame.color=new Color(165,42,42);
                break;
            case "Orange":
                cardFrame.color=new Color(255,165,0);
                break;
            case "Black":
                cardFrame.color=new Color(0,0,0);
                break;
            case "Gold":
                cardFrame.color=new Color(218,165,32);
                break;
            default:
                cardFrame.color=new Color(255,255,255);
                break;
        }
    }
    private void CheckBackCard()
    {
        if(isCardBack)
            m_CardBack.SetActive(true);
        else
            m_CardBack.SetActive(false);
    }
    public void SetCard()
    {
        if(HealthManager.Instance.playerCurrentMana<cost || GameManager.Instance.state!=BattleState.PLAYERTURN)
            return;
        if(GameObject.Find("MyActiveCard").transform.childCount>=6)
        {
            LogManager.Instance.Log("Can not use more than 6 cards on state");
            return;
        }
        GameObject ActiveCard=GameObject.Find("MyActiveCard");
        GameObject newCard = Instantiate(Card,ActiveCard.transform.position,ActiveCard.transform.rotation);
        newCard.GetComponent<CardDetail>().Id=GetComponent<CardDetail>().Id;
        switch (GetComponent<CardDetail>().Type)
        {
            case "Summon":
                LogManager.Instance.Log("Player has summon "+newCard.GetComponent<CardDetail>().cardName);
                break;
            case "Magic":
                switch (GetComponent<CardDetail>().cardName)
                {
                    case "Fireball":
                        HealthManager.Instance.RivalTookDmg(GetComponent<CardDetail>().power);
                        break;
                    case "Healing":
                        HealthManager.Instance.PlayerHeal(GetComponent<CardDetail>().power);
                        break;
                    case "Boost Attack":
                        if(GameManager.Instance.myActiveCard.transform.childCount<=0)
                        {
                            LogManager.Instance.Log("<color=#ff0000>There are no your active card </color>");
                            Destroy(newCard);
                            return;
                        }
                        GameObject[] Target = new GameObject[GameManager.Instance.myActiveCard.transform.childCount];
                        for (int i = 0; i < Target.Length; i++)
                        {
                            Target[i]=GameManager.Instance.myActiveCard.transform.GetChild(i).gameObject;
                        }       
                        int x=UnityEngine.Random.Range(0,Target.Length-1);
                        Target[x].GetComponent<CardDetail>().power+=GetComponent<CardDetail>().power;
                        break;
                    case "Reduce Attack":
                        if(GameManager.Instance.rivalActiveCard.transform.childCount<=0)
                        {
                            LogManager.Instance.Log("<color=#ff0000>There are no rival active card</color>");
                            Destroy(newCard);
                            return;
                        }
                        GameObject[] target = new GameObject[GameManager.Instance.rivalActiveCard.transform.childCount];
                        for (int i = 0; i < target.Length; i++)
                        {
                            target[i]=GameManager.Instance.rivalActiveCard.transform.GetChild(i).gameObject;
                        }       
                        int y=UnityEngine.Random.Range(0,target.Length-1);
                        target[y].GetComponent<CardDetail>().power-=GetComponent<CardDetail>().power;
                        break;
                    case "Steal Card":
                        if(GameManager.Instance.rivalCardDeck.transform.childCount<=0 && GameManager.Instance.myCardDeck.transform.childCount<5)
                        {
                            LogManager.Instance.Log("<color=#ff0000>Can not steal the rival cards </color>");
                            Destroy(newCard);
                            return;
                        }
                        GameObject[] m_Target = new GameObject[GameManager.Instance.rivalCardDeck.transform.childCount];
                        for (int i = 0; i < m_Target.Length; i++)
                        {
                            m_Target[i]=GameManager.Instance.rivalCardDeck.transform.GetChild(i).gameObject;
                        }       
                        for (int i = 0; i < GetComponent<CardDetail>().power; i++)
                        {
                            int n=UnityEngine.Random.Range(0,m_Target.Length-1);
                            GameObject m_newCard = Instantiate(c_Card,GameManager.Instance.myCardDeck.transform.position,GameManager.Instance.myCardDeck.transform.rotation);
                            m_newCard.GetComponent<CardDetail>().Id=m_Target[n].GetComponent<CardDetail>().Id;
                            m_newCard.transform.SetParent(GameManager.Instance.myCardDeck.transform);
                            Destroy(m_Target[n]);    
                        }
                        break;
                    case "Destroy Card":
                        if(GameManager.Instance.rivalCardDeck.transform.childCount<=0)
                        {
                            LogManager.Instance.Log("<color=#ff0000>There are no card to Destroy </color>");
                            Destroy(newCard);
                            return;
                        }
                        GameObject[] n_Target = new GameObject[GameManager.Instance.rivalCardDeck.transform.childCount];
                        for (int i = 0; i < n_Target.Length; i++)
                        {
                            n_Target[i]=GameManager.Instance.rivalCardDeck.transform.GetChild(i).gameObject;
                        }       
                        for (int i = 0; i < GetComponent<CardDetail>().power; i++)
                        {
                            int z=UnityEngine.Random.Range(0,n_Target.Length-1);
                            Destroy(n_Target[z]);    
                        }
                        break;
                    case "Bonus":
                        HealthManager.Instance.PlayerMana(GetComponent<CardDetail>().power);
                        break;
                }
                LogManager.Instance.Log("Player has use "+GetComponent<CardDetail>().cardName+" magic card");
                break;
        }
        newCard.transform.SetParent(ActiveCard.transform);
        GameManager.Instance.GetComponent<AudioSource>().PlayOneShot(MusicManager.Instance.a_setCard);
        if(GetComponent<CardDetail>().Type=="Magic")
            newCard.GetComponent<CardDetail>().StartCoroutine(DestroyCard(newCard));
        HealthManager.Instance.playerCurrentMana-=cost;
        Destroy(gameObject);
    }
    public IEnumerator DestroyCard(GameObject card)
    {
        yield return new WaitForSeconds(1f);
        Destroy(card);
    }
}
