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
            String m = "" + month.ToString("00");
            String y = "" + year.ToString("0000");
            text.text = m + "/" + y;
        } else {
            text.text = "";
        }
    }
}
