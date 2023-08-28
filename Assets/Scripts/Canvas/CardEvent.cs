using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardEvent : MonoBehaviour
{
    public Button EndTurn_btn;
    private void Awake() =>EndTurn_btn.onClick.AddListener(()=>{GameManager.Instance.EndTurn();});
}
