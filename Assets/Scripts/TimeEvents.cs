using UnityEngine;

public class TimeEvents : MonoBehaviour
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
