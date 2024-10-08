using Unity.VisualScripting;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Time")]
    public int day = 1;
    public int month = 1;
    public int year = 1;

    [Header("Time Settings")]
    [Range(0.01f, 5f)]
    public float dayLength = 1f;

    [Header("Info")]


    // Private variables
    private float currentTime = 1f; // Tracks the current time in seconds between days
    private bool timerStart = false; // Makes sure the timers start when worldGen is finished
    private TimeEvents events; // Events such as day updates

    void Start(){
        currentTime = dayLength;
        // Subscribes timer to onWorldgenFinishEvent
        WorldgenEvents.onWorldgenFinished += startTimers;
        // Gets event component
        events = GetComponent<TimeEvents>();
    }
    void startTimers(){
        // Starts timers when worldgen is finished
        timerStart = true;
    }
    void Update()
    {
        if (timerStart){
            currentTime -= Time.deltaTime;
            if (currentTime <= 0){
                currentTime = dayLength;
                incrementDay();
            }
        }
        
    }

    void incrementDay(){
        day++;
        events.updateDay();
        if (day > 30){
            day = 1;
            month++;
            if (month > 12){
                month = 1;
                year++;
            }
        }
    }
}
