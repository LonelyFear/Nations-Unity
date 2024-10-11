using UnityEngine;

public class TimeEvents : MonoBehaviour
{
    public delegate void DayFinished();
    public static event DayFinished dayUpdate;

    public delegate void MonthFinished();
    public static event MonthFinished monthUpdate;
    
    public delegate void YearFinished();
    public static event YearFinished yearUpdate;
    
    public void updateDay(){
        if (dayUpdate != null){
            dayUpdate();
        }
    }

    public void updateMonth(){
        if (monthUpdate != null){
            monthUpdate();
        }
    }

    public void updateYear(){
        if (yearUpdate != null){
            yearUpdate();
        }
    }
}
