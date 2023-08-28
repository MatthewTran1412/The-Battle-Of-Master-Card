using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    public static MusicManager Instance{get=>instance;}
    public AudioClip a_PlayerWin;
    public AudioClip a_RivalWin;
    public AudioClip a_drawCard;
    public AudioClip a_setCard;
    public AudioClip[] a_GameMusic;
    private string oldsong;
    public AudioClip a_PlayerTurn;
    public int i;

    [SerializeField] private float musictimer;
    [SerializeField] private float musiccd;

    private void Awake() {
        if(instance)
            Debug.LogError("More than 1 Music Manager");
        instance=this;
    }
    private void OnEnable() 
    {
        if(GameManager.Instance)
            GameManager.Instance.e_DuringGame+=GameMusic;
    }
    private void OnDisable(){
        if(GameManager.Instance)
            GameManager.Instance.e_DuringGame-=GameMusic;
    }
    private void Start(){
        i=0;
        StartCoroutine(StartMusic());
        musictimer=Time.time;
        musiccd=a_GameMusic[i].length+15;
    }
    private void Update() {
        if(i<0)
            i=a_GameMusic.Length-1;
        else if(i>a_GameMusic.Length-1)
            i=0;
    }
    public void Minusi()=>i--;
    public void Addi()=>i++;
    private void GameMusic()
    {
        if(musiccd<Time.time-musictimer)
            GetComponent<AudioSource>().PlayOneShot(a_GameMusic[i]);
    }
    private IEnumerator StartMusic()
    {
        yield return new WaitForSeconds(1);
        oldsong=a_GameMusic[i].name;
        GetComponent<AudioSource>().PlayOneShot(a_GameMusic[i]);
    }
    public void SaveAndPlay(){
        if(a_GameMusic[i].name!=oldsong)
        {
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().PlayOneShot(a_GameMusic[i]);    
            oldsong=a_GameMusic[i].name;
        }
    }
}
