using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthManager : MonoBehaviour
{
    private static HealthManager instance;
    public static HealthManager Instance{get=>instance;}
    public int maxhp;
    public int playerCurrenthp;
    public int rivalCurrenthp;
    public int maxMana;
    public int playerCurrentMana;
    public int rivalCurrentMana;
    public Text t_playerhp;
    public Text t_rivalhp;
    public Text t_playercost;
    private void Awake() {
        if(instance)
            Debug.LogError("More than 1 Health Manager");
        instance=this;
    }
    void Start()
    {
        playerCurrenthp=rivalCurrenthp=maxhp;
        playerCurrentMana=rivalCurrentMana=1;
    }
    public void PlayerTookDmg(int amount)=>playerCurrenthp-=amount;
    public void RivalTookDmg(int amount)=>rivalCurrenthp-=amount;
    public void PlayerHeal(int amount)=>playerCurrenthp+=amount;
    public void RivalHeal(int amount)=>rivalCurrenthp+=amount;
    public void PlayerMana(int amount)=>playerCurrentMana+=amount;
    public void RivalMana(int amount)=>rivalCurrentMana+=amount;
    private void Update(){
        t_playerhp.text=playerCurrenthp.ToString();
        t_rivalhp.text=rivalCurrenthp.ToString();
        t_playercost.text=playerCurrentMana.ToString();
        LimitMana();
        LimitHealth();
    }
    private void LimitMana()
    {
        if(playerCurrentMana>10)
            playerCurrentMana=10;
        if(playerCurrentMana<0)
            playerCurrentMana=0;
        if(rivalCurrentMana>10)
            playerCurrentMana=10;
        if(rivalCurrentMana<0)
            playerCurrentMana=0;
    }
    private void LimitHealth()
    {
        if(playerCurrenthp>maxhp)
            playerCurrenthp=maxhp;
        if(playerCurrenthp<=0)
            playerCurrenthp=0;
        if(rivalCurrenthp>maxhp)
            rivalCurrenthp=maxhp;
        if(rivalCurrenthp<=0)
            rivalCurrenthp=0;
    }
}
