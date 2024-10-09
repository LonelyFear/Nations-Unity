using UnityEngine;

public class WorldgenEvents : MonoBehaviour
{
    public delegate void WorldgenDone();
    public static event WorldgenDone onWorldgenFinished;
    public void worldgenFinish()
    {
        if (onWorldgenFinished != null){
            print("Worldgen finished");
            onWorldgenFinished();
        }
    }
}
