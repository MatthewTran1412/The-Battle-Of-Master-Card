using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class SettingSystem : MonoBehaviour
{
    private static SettingSystem instance;
    public static SettingSystem Instance{get=>instance;}
    [SerializeField] private Button Settingbtn;
    [Header("Main Panel")]
    public GameObject m_SettingSystem;
    [SerializeField] private Button Close;

    [SerializeField] private Button StartGame;
    [SerializeField] private Button Back;
    [SerializeField] private Button System;
    [SerializeField] private Button Menu;
    [SerializeField] private Button Exit;

    [Header("Sub Panel")]
    public GameObject m_System;
    [SerializeField] private Button s_Close;
    [SerializeField] private Button Previous;
    [SerializeField] private Button Next;
    [SerializeField] private Text MusicName;
    [SerializeField] private Slider s_Music;
    [SerializeField] private Slider s_SFX;
    [SerializeField] private Toggle t_Music;
    [SerializeField] private Toggle t_SFX;
    [SerializeField]private AudioMixer m_Mixer;
    // Start is called before the first frame update
    private void Awake() {
        if(StartGame!=null)
        {
            StartGame.onClick.AddListener(()=>{SceneManager.LoadScene("Game");});
        }
        if(Settingbtn!=null)
        {
            Settingbtn.onClick.AddListener(()=>{
                Time.timeScale=0f;
                m_SettingSystem.SetActive(true);
            });
        }
        if(Close!=null)
        {
            Close.onClick.AddListener(()=>{
            Time.timeScale=1f;
            m_SettingSystem.SetActive(false);
            });
        }
        if(Back!=null)
        {
            Back.onClick.AddListener(()=>{
            Time.timeScale=1f;
            m_SettingSystem.SetActive(false);
            });
        }
        if(System!=null)
        {
            System.onClick.AddListener(()=>{
            if(m_SettingSystem!=null)
                m_SettingSystem.SetActive(false);
            m_System.SetActive(true);
            });
        }
        if(Menu!=null)
            Menu.onClick.AddListener(()=>{SceneManager.LoadScene("Menu");});
        if(Exit!=null)
            Exit.onClick.AddListener(()=>{Application.Quit();});
        
        s_Close.onClick.AddListener(()=>{
            if(SceneManager.GetActiveScene().name=="Game")
                m_SettingSystem.SetActive(true);
            m_System.SetActive(false);
            MusicManager.Instance.SaveAndPlay();
            SaveSetting();
        });
        if(Previous!=null)
        {
            Previous.onClick.AddListener(()=>{MusicManager.Instance.Minusi();});
            Next.onClick.AddListener(()=>{MusicManager.Instance.Addi();});
        }
        if(t_Music!=null)
            t_Music.onValueChanged.AddListener((t)=>t_Music.isOn=t);
        if(t_SFX!=null)
            t_SFX.onValueChanged.AddListener((t)=>t_SFX.isOn=t);
    }
    void Start(){
        if(!PlayerPrefs.HasKey("musicvolume")) PlayerPrefs.SetFloat("musicvolume",.7f);
        if(!PlayerPrefs.HasKey("SFXvolume")) PlayerPrefs.SetFloat("SFXvolume",.7f);
        if(!PlayerPrefs.HasKey("musicmute")) PlayerPrefs.SetInt("musicmute",0);
        if(!PlayerPrefs.HasKey("SFXmute")) PlayerPrefs.SetInt("SFXmute",0);
        if(SceneManager.GetActiveScene().name=="Game")
            m_SettingSystem.SetActive(false);
        m_System.SetActive(false);
        VolumeSetting();
    }
    // Update is called once per frame
    private void Update() {
        if(MusicName!=null)
            MusicName.text=MusicManager.Instance.a_GameMusic[MusicManager.Instance.i].name;
        ChangeVolume();
    }
    public void ChangeVolume()
    {
        s_Music.value=t_Music.isOn?0:s_Music.value;
        s_SFX.value=t_SFX.isOn?0:s_SFX.value;
        m_Mixer.SetFloat("musicvolume",t_Music.isOn?-80f:Mathf.Log10(s_Music.value)*20);
        m_Mixer.SetFloat("SFXvolume",t_SFX.isOn?-80f:Mathf.Log10(s_SFX.value)*20);
        SaveSetting();
    }
    private void VolumeSetting()
    {
        t_Music.isOn=PlayerPrefs.GetInt("musicmute")==1?true:false;
        t_SFX.isOn=PlayerPrefs.GetInt("SFXmute")==1?true:false;
        s_Music.value=t_Music.isOn?0:PlayerPrefs.GetFloat("musicvolume");
        s_SFX.value=t_SFX.isOn?0:PlayerPrefs.GetFloat("SFXvolume");
    }
    private void SaveSetting(){
        PlayerPrefs.SetInt("musicmute",t_Music.isOn?1:0);
        PlayerPrefs.SetInt("SFXmute",t_SFX.isOn?1:0);
        PlayerPrefs.SetFloat("musicvolume",s_Music.value);
        PlayerPrefs.SetFloat("SFXvolume",s_SFX.value);
    }
}
