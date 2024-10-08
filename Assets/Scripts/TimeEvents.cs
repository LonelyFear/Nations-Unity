using UnityEngine;

public class TimeEvents : MonoBehaviour
{
    public delegate void DayFinished();
    public static event DayFinished dayUpdate;
    
    public void updateDay(){
        if (dayUpdate != null){
            dayUpdate();
        }
    }
}
