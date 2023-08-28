using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
public enum BattleState { START, PLAYERTURN, RIVALTURN, WON, LOST }
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance{get=>instance;}
    public event Action e_SetUp;
    public event Action e_GamePlay;
    public event Action e_DuringGame;
    [Header("My")]
    [SerializeField] public GameObject myCardDeck;
    [SerializeField] public GameObject myDeck;
    [SerializeField] public GameObject myActiveCard;

    [Header("Rival")]
    [SerializeField] public GameObject rivalCardDeck;
    [SerializeField] public GameObject rivalDeck;
    [SerializeField] public GameObject rivalActiveCard;

    [Header("Card")]
    [SerializeField] private GameObject cardPrefbs;
    [SerializeField] private GameObject backCardPrefbs;

    [Header("Announcement")]
    [SerializeField] private Text announcement;
    [SerializeField] private Button replay;
    [SerializeField] private float timer;
    [SerializeField] private Text timerText;

    private AudioSource audio;
    public BattleState state;
    private void Awake() {
        if(instance)
            Debug.LogError("More than one Game Manager");
        instance=this;
        replay.onClick.AddListener(()=>{
            SceneManager.LoadScene("Game");
        });
    }
    private void OnEnable() {
        e_SetUp+=DeckSetUp;
        e_SetUp+=CardDeckSetUp;
        e_GamePlay+=GamePlayed;
        e_DuringGame+=CheckWin;
    }
    private void OnDisable() {
        e_SetUp-=DeckSetUp;
        e_SetUp-=CardDeckSetUp;
        e_GamePlay-=GamePlayed;
        e_DuringGame-=CheckWin;
    }
    private void Start() {
        replay.gameObject.SetActive(false);
        audio=GetComponent<AudioSource>();
        timer=60;
        state=BattleState.START;
        e_SetUp?.Invoke();
        e_GamePlay?.Invoke();
    }
    private void Update() {
        timerText.text=state==BattleState.PLAYERTURN?Mathf.Round(timer).ToString():"";
        timer-=state==BattleState.PLAYERTURN?Time.deltaTime:0;
        if(timer<=0)
            StartCoroutine(EndPlayerTurn());
        e_DuringGame?.Invoke();
    }
    private void DeckSetUp()
    {
        for (int i = 0; i < 4; i++)
        {
            if (myDeck.transform.childCount<4)
            {
                GameObject mycard = Instantiate(backCardPrefbs,myDeck.transform.position,myDeck.transform.rotation);    
                mycard.transform.localScale= Vector3.one;
                mycard.transform.SetParent(myDeck.transform);
            }
            if(rivalDeck.transform.childCount<4)
            {
                GameObject rivalcard = Instantiate(backCardPrefbs,rivalDeck.transform.position,rivalDeck.transform.rotation);
                rivalcard.transform.localScale= Vector3.one;
                rivalcard.transform.SetParent(rivalDeck.transform);
            }
        }
    }
    private void CardDeckSetUp()
    {
        for (int i = 0; i < 5; i++)
        {
            if (myCardDeck.transform.childCount<5)
            {
                GameObject mycard = Instantiate(cardPrefbs,myCardDeck.transform.position,myCardDeck.transform.rotation);    
                mycard.transform.localScale= Vector3.one;
                mycard.transform.SetParent(myCardDeck.transform);
                mycard.GetComponent<CardDetail>().Id=UnityEngine.Random.Range(1,CardDB.Instance.cardList.Length);
            }
            if(rivalCardDeck.transform.childCount<5)
            {
                GameObject rivalcard = Instantiate(backCardPrefbs,rivalCardDeck.transform.position,rivalCardDeck.transform.rotation);
                rivalcard.transform.localScale= Vector3.one;
                rivalcard.transform.SetParent(rivalCardDeck.transform);
                rivalcard.GetComponent<CardDetail>().Id=UnityEngine.Random.Range(1,CardDB.Instance.cardList.Length);
            }
        }
    }
    private void DrawCard(GameObject Deck,Transform CardDeck,GameObject Prefbs )
    {
        if(Deck.GetComponent<Deck>().deckSize<=0)
            return;
        Deck.GetComponent<Deck>().Shuffle();
        GameObject newCard = Instantiate(Prefbs,CardDeck.position,CardDeck.rotation);
        newCard.GetComponent<CardDetail>().Id=Deck.GetComponent<Deck>().deck[0].id;
        RemoveAt(ref Deck.GetComponent<Deck>().deck,0);
        Deck.GetComponent<Deck>().deckSize--;
        newCard.transform.SetParent(CardDeck);
        audio.PlayOneShot(MusicManager.Instance.a_drawCard);
    }
    public static void RemoveAt<T>(ref T[] arr, int index)
    {
        for (int a = index; a < arr.Length - 1; a++)
        {
            arr[a] = arr[a + 1];
        }
        Array.Resize(ref arr, arr.Length - 1);
    }
    private void GamePlayed()
    {
        LogManager.Instance.Log("Game Start!!!!!");
        StartCoroutine(PlayerTurn());
    }
    private IEnumerator PlayerTurn()
    {
        state=BattleState.PLAYERTURN;
        audio.PlayOneShot(MusicManager.Instance.a_PlayerTurn);
        LogManager.Instance.Log("Player Turn");
        yield return new WaitForSeconds(.5f);
        HealthManager.Instance.playerCurrentMana++;
        if(myCardDeck.transform.childCount<9)
            DrawCard(myDeck,myCardDeck.transform,cardPrefbs);
    }
    private IEnumerator EndPlayerTurn()
    {
        SummonCardAction(myActiveCard.transform,rivalActiveCard.transform,state);
        timerText.text="";
        state=BattleState.RIVALTURN;
        timer=1000;
        yield return new WaitForSeconds(2f);
        timer=60;
        StartCoroutine(RivalTurn());
    }
    private IEnumerator RivalTurn()
    {
        yield return new WaitForSeconds(1f);
        LogManager.Instance.Log("Rival Turn");
        yield return new WaitForSeconds(1f);
        HealthManager.Instance.rivalCurrentMana++;
        if(rivalCardDeck.transform.childCount<9)
            DrawCard(rivalDeck,rivalCardDeck.transform,backCardPrefbs);
        yield return new WaitForSeconds(1);
        int x=-1;
        GameObject newCard=null;
        if(rivalActiveCard.transform.childCount<6)
        {
            for (int i = 0; i < rivalCardDeck.transform.childCount; i++)
            {
                if(rivalCardDeck.transform.GetChild(i).gameObject.GetComponent<CardDetail>().cost<=HealthManager.Instance.rivalCurrentMana)
                {
                    if(rivalCardDeck.transform.GetChild(i).gameObject.GetComponent<CardDetail>().Type=="Summon" || rivalCardDeck.transform.GetChild(i).gameObject.GetComponent<CardDetail>().cardName=="Reduce Attack" && myActiveCard.transform.childCount>0||rivalCardDeck.transform.GetChild(i).gameObject.GetComponent<CardDetail>().cardName=="Boost Attack" && rivalActiveCard.transform.childCount>0 ||rivalCardDeck.transform.GetChild(i).gameObject.GetComponent<CardDetail>().cardName=="Fireball"||rivalCardDeck.transform.GetChild(i).gameObject.GetComponent<CardDetail>().cardName=="Healing")
                    {
                        x=i;
                        break;
                    }
                }
            }
            if(x>-1 && x<rivalCardDeck.transform.childCount-1)
            {
                newCard =Instantiate(cardPrefbs,rivalActiveCard.transform.position,rivalActiveCard.transform.rotation);
                newCard.transform.SetParent(rivalActiveCard.transform);
                newCard.GetComponent<CardDetail>().Id=rivalCardDeck.transform.GetChild(x).gameObject.GetComponent<CardDetail>().Id;
                HealthManager.Instance.rivalCurrentMana-=rivalCardDeck.transform.GetChild(x).gameObject.GetComponent<CardDetail>().cost;
                if(rivalCardDeck.transform.GetChild(x).gameObject.GetComponent<CardDetail>().Type=="Summon")
                    LogManager.Instance.Log("Rival has summon"+newCard.GetComponent<CardDetail>().cardName);
                else if(rivalCardDeck.transform.GetChild(x).gameObject.GetComponent<CardDetail>().Type=="Magic")
                    LogManager.Instance.Log("Rival has use "+newCard.GetComponent<CardDetail>().cardName+" magic card");
                Destroy(rivalCardDeck.transform.GetChild(x).gameObject);
            }
        }
        yield return new WaitForSeconds(1f);
        if(newCard!=null)
        {
            switch (newCard.GetComponent<CardDetail>().Type)
            {
                case "Summon":
                    SummonCardAction(rivalActiveCard.transform,myActiveCard.transform,state);
                    break;
                case "Magic":
                    MagicCardAction(rivalActiveCard.transform,myActiveCard.transform,state,newCard);
                    break;
            }    
            audio.PlayOneShot(MusicManager.Instance.a_setCard);
        }
        else
            Destroy(newCard);
        yield return new WaitForSeconds(1f);
        state=BattleState.PLAYERTURN;
        StartCoroutine(PlayerTurn());
    }
    private void MagicCardAction(Transform AttackerCardDeck,Transform DefenderCardDeck,BattleState state,GameObject card) 
    {
        switch (card.GetComponent<CardDetail>().cardName)
        {
            case "Fireball":
                    HealthManager.Instance.PlayerTookDmg(card.GetComponent<CardDetail>().power);
                    LogManager.Instance.Log(card.GetComponent<CardDetail>().cardName+" Dealing "+card.GetComponent<CardDetail>().power+" to player");
                    break;
            case "Healing":
                    HealthManager.Instance.RivalHeal(card.GetComponent<CardDetail>().power);
                    LogManager.Instance.Log(card.GetComponent<CardDetail>().cardName+card.GetComponent<CardDetail>().power+" to hisself");
                    break;
            case "Boost Attack":
                if(AttackerCardDeck.childCount<=0)
                    return;
                GameObject[] Target = new GameObject[AttackerCardDeck.childCount];
                for (int i = 0; i < Target.Length; i++)
                {
                    Target[i]=AttackerCardDeck.GetChild(i).gameObject;
                }
                int x=UnityEngine.Random.Range(0,Target.Length-1);
                Target[x].GetComponent<CardDetail>().power+=card.GetComponent<CardDetail>().power;
                LogManager.Instance.Log(card.GetComponent<CardDetail>().cardName+" boosted attack to "+Target[x].GetComponent<CardDetail>().cardName);
                break;
            case "Reduce Attack":
                if(AttackerCardDeck.childCount<=0)
                    return;
                GameObject[] target = new GameObject[DefenderCardDeck.childCount];
                for (int i = 0; i < target.Length; i++)
                {
                    target[i]=DefenderCardDeck.GetChild(i).gameObject;
                }
                int y=UnityEngine.Random.Range(0,target.Length-1);
                target[y].GetComponent<CardDetail>().power-=card.GetComponent<CardDetail>().power;
                LogManager.Instance.Log(card.GetComponent<CardDetail>().cardName+" debuff attack to "+target[y].GetComponent<CardDetail>().cardName);
                break;
            case "Steal Card":
                if(DefenderCardDeck.transform.childCount<=0 &&AttackerCardDeck.transform.childCount<5)
                {
                    //LogManager.Instance.Log("<color=#ff0000>Can not steal the rival cards </color>");
                    Destroy(card);
                    return;
                }
                GameObject[] m_Target = new GameObject[DefenderCardDeck.transform.childCount];
                for (int i = 0; i < m_Target.Length; i++)
                {
                    m_Target[i]=DefenderCardDeck.transform.GetChild(i).gameObject;
                }       
                for (int i = 0; i < GetComponent<CardDetail>().power; i++)
                {
                    int n=UnityEngine.Random.Range(0,m_Target.Length-1);
                    GameObject m_newCard = Instantiate(backCardPrefbs,AttackerCardDeck.transform.position,AttackerCardDeck.transform.rotation);
                    m_newCard.GetComponent<CardDetail>().Id=m_Target[n].GetComponent<CardDetail>().Id;
                    m_newCard.transform.SetParent(AttackerCardDeck.transform);
                    Destroy(m_Target[n]);    
                }
                break;
            case "Destroy Card":
                if(DefenderCardDeck.transform.childCount<=0)
                {
                    //LogManager.Instance.Log("<color=#ff0000>There are no card to Destroy </color>");
                    Destroy(card);
                    return;
                }
                GameObject[] n_Target = new GameObject[DefenderCardDeck.transform.childCount];
                for (int i = 0; i < n_Target.Length; i++)
                {
                    n_Target[i]=DefenderCardDeck.transform.GetChild(i).gameObject;
                }       
                for (int i = 0; i < GetComponent<CardDetail>().power; i++)
                {
                    int z=UnityEngine.Random.Range(0,n_Target.Length-1);
                    Destroy(n_Target[z]);    
                }
                break;
            case "Bonus":
                HealthManager.Instance.RivalMana(GetComponent<CardDetail>().power);
                break;
        }
        StartCoroutine(DestroyObject(card));
    }
    private void SummonCardAction(Transform AttackerCardDeck,Transform DefenderCardDeck,BattleState state)
    {
        if(AttackerCardDeck.childCount<=0)
            return;
        GameObject[] Target = new GameObject[DefenderCardDeck.childCount];
        for (int i = 0; i < Target.Length; i++)
        {
            Target[i]=DefenderCardDeck.GetChild(i).gameObject;
        }
        for (int i = 0; i < AttackerCardDeck.childCount; i++)
        {
            GameObject t=Target.FirstOrDefault(t=>t.GetComponent<CardDetail>().power<=AttackerCardDeck.GetChild(i).GetComponent<CardDetail>().power);
            if(t)
            {
                Destroy(t);
                if(AttackerCardDeck.GetChild(i).GetComponent<CardDetail>().power>t.GetComponent<CardDetail>().power)
                    LogManager.Instance.Log(AttackerCardDeck.GetChild(i).GetComponent<CardDetail>().cardName+" has destroyed "+t.GetComponent<CardDetail>().cardName);
                else if(AttackerCardDeck.GetChild(i).GetComponent<CardDetail>().power==t.GetComponent<CardDetail>().power)
                {
                    Destroy(AttackerCardDeck.GetChild(i).gameObject);
                    LogManager.Instance.Log(AttackerCardDeck.GetChild(i).GetComponent<CardDetail>().cardName+" has destroyed "+t.GetComponent<CardDetail>().cardName);
                }
            }
            else
            {
                if(DefenderCardDeck.childCount<=0)
                {
                    if(state==BattleState.PLAYERTURN)
                    {
                        LogManager.Instance.Log(AttackerCardDeck.GetChild(i).GetComponent<CardDetail>().cardName+" deal "+AttackerCardDeck.GetChild(i).GetComponent<CardDetail>().power+" to rival");
                        HealthManager.Instance.RivalTookDmg(AttackerCardDeck.GetChild(i).GetComponent<CardDetail>().power);
                    }
                    else if(state==BattleState.RIVALTURN)
                    {
                        LogManager.Instance.Log(AttackerCardDeck.GetChild(i).GetComponent<CardDetail>().cardName+" deal "+AttackerCardDeck.GetChild(i).GetComponent<CardDetail>().power+" to player");
                        HealthManager.Instance.PlayerTookDmg(AttackerCardDeck.GetChild(i).GetComponent<CardDetail>().power);
                    }
                }
            }
        }
    }
    public void EndTurn()=>timer-=state==BattleState.PLAYERTURN?timer:0;
    private IEnumerator DestroyObject(GameObject target)
    {
        yield return new WaitForSeconds(1f);
        Destroy(target);
    }
    private void CheckWin()
    {
        if(HealthManager.Instance.rivalCurrenthp<=0)
        {
            audio.PlayOneShot(MusicManager.Instance.a_PlayerWin);
            announcement.text="<color=#FFDF00>You Win</color>";
            Time.timeScale=0;
            replay.gameObject.SetActive(true);
        }
        else if(HealthManager.Instance.playerCurrenthp<=0)
        {
            audio.PlayOneShot(MusicManager.Instance.a_RivalWin);
            announcement.text="<color=#A9A9A9>You Lose</color>";
            Time.timeScale=0;
            replay.gameObject.SetActive(true);
        }
    }
}
