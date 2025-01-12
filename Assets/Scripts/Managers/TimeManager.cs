using System;
using Unity.VisualScripting;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Time")]
    public int ticks = 0;
    public float month = 1f;
    public int year = 1;

    [Header("Time Settings")]
    [Range(0.05f, 1f)]
    [SerializeField]
    float tickLength = 0.05f;
    public static int ticksPerYear = 12;


    [Header("Info")]
    public bool timerStart = false; // Makes sure the timers start when worldGen is finished
    // Private variables
    private float currentTime = 1f; // Tracks the current time in seconds between ticks

    public bool paused { get; private set; } = false;

    void Start(){
        currentTime = tickLength;
    }
    public void startTimers(){
        // Starts timers when worldgen is finished
        timerStart = true;
    }
    void Update()
    {
        year = ticks / ticksPerYear;
        if (timerStart && !paused){
            currentTime -= Time.deltaTime;
            if (currentTime <= 0){
                currentTime = tickLength;
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

        month += 12 / ticksPerYear;
        ticks += 1;
        Events.TickGame();

        if (month > 12){
            month = 1;
            year++;
            Events.UpdateYear();
        }
    }

    public void Pause(){
        paused = true;
    }

    public void Resume(){
        paused = false;
    }
}

