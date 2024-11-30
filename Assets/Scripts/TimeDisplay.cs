using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{
    TimeManager tm;
    TextMeshProUGUI text; 

    void Start(){
        text = GetComponent<TextMeshProUGUI>();
        tm = FindAnyObjectByType<TimeManager>();
        text.text = "";
    }
    void Update(){
        if (tm.timerStart){
            gameObject.SetActive(true);
            
            int month = Mathf.RoundToInt(tm.month);
            int year = Mathf.RoundToInt(tm.year);

            String m = "" + month.ToString("00");
            String y = "" + Mathf.Abs(year).ToString("0000");
            text.text = y;
        } else {
            text.text = "";
        }
    }
}
