using UnityEditor;

public class Events{
    public delegate void Tick();
    public static event Tick tick;
    public delegate void YearFinished();
    public static event YearFinished yearUpdate;
    public delegate void PopTick();    
    public static event PopTick popTick;
    public delegate void TileTick();    
    public static event TileTick tileTick;
    public delegate void StateTick();
    public static event StateTick stateTick;
    public static void TickGame(){
        if (tick != null){
            tick();
        }
    }
    public static void UpdateYear(){
        if (yearUpdate != null){
            yearUpdate();
        }
    }
    public static void TickPops(){
        if (popTick != null){
            popTick();
        }
    }
    public static void TickTiles(){
        if (tileTick != null){
            tileTick();
        }
    }    
    public static void TickStates(){
        if (stateTick != null){
            stateTick();
        }
    }    
}