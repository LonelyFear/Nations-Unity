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
    [SerializeField]
    int ticksPerYear = 4;


    [Header("Info")]
    public bool timerStart = false; // Makes sure the timers start when worldGen is finished
    // Private variables
    private float currentTime = 1f; // Tracks the current time in seconds between ticks
    
    private TimeEvents events; // event class instance

    public bool paused { get; private set; } = false;

    void Start(){
        currentTime = tickLength;
        // Gets event component
        events = new TimeEvents();
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
        events.TickGame();
        if (month - Math.Floor(month) == 0){
            events.UpdateMonth();
        }

        if (month > 12){
            month = 1;
            year++;
            events.UpdateYear();
        }
    }

    public void Pause(){
        paused = true;
    }

    public void Resume(){
        paused = false;
    }
}
public class TimeEvents
{
    public delegate void Tick();
    public static event Tick tick;

    public delegate void MonthFinished();
    public static event MonthFinished monthUpdate;
    
    public delegate void YearFinished();
    public static event YearFinished yearUpdate;
    
    public void TickGame(){
        if (tick != null){
            tick();
        }
    }

    public void UpdateMonth(){
        if (monthUpdate != null){
            monthUpdate();
        }
    }

    public void UpdateYear(){
        if (yearUpdate != null){
            yearUpdate();
        }
    }
}
