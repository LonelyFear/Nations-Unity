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
            int day = tm.day;
            int month = tm.month;
            int year = tm.year;
            String d = "" + day.ToString("00");
            String m = "" + month.ToString("00");
            text.text = d + "/" + m + "/" + year;
        } else {
            text.text = "";
        }
    }
}
