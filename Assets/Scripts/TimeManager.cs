using Unity.VisualScripting;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Time")]
    public int day = 1;
    public int month = 1;
    public int year = 1;

    [Header("Time Settings")]
    [Range(0f, 0.5f)]
    public float dayLength = 0.05f;

    [Header("Info")]
    public bool timerStart = false; // Makes sure the timers start when worldGen is finished
    // Private variables
    private float currentTime = 1f; // Tracks the current time in seconds between days
    
    private TimeEvents events; // Events such as day updates

    public bool paused { get; private set; } = false;

    void Start(){
        currentTime = dayLength;
        // Gets event component
        events = GetComponent<TimeEvents>();
    }
    public void startTimers(){
        // Starts timers when worldgen is finished
        timerStart = true;
    }
    void Update()
    {
        if (timerStart && !paused){
            currentTime -= Time.deltaTime;
            if (currentTime <= 0){
                currentTime = dayLength;
                IncrementTime();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space)){
            if (paused){
                Resume();
            } else {
                Pause();
            }
        }
    }

    void IncrementTime(){
        month++;
        events.updateMonth();
        if (month > 12){
            month = 1;
            year++;
            events.updateYear();
        }
    }

    public void Pause(){
        paused = true;
    }

    public void Resume(){
        paused = false;
    }
}

