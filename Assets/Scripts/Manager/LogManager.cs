using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    private static LogManager instance;
    public static LogManager Instance{get=>instance;}
    [SerializeField] private Text LogText;

    private void Awake() {
        if(instance)
            Debug.LogError("More than one log manager");
        instance=this;
    }
    public void Log(string Content)=>LogText.text=Content+"\n\n"+LogText.text;
}
