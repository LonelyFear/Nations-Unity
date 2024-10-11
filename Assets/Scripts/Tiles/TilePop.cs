using Unity.VisualScripting;
using UnityEngine;

public class TilePop : MonoBehaviour
{
    // Base pop growth per month
    public const float basePopGrowth = 0.0008f;
    public int population;
    public float integration;
    public int troops;

    Tile tile;

    public void initTilePop(){
        tile = GetComponent<Tile>();
        // Initializes tile population
        if (!tile.terrain.naval){
            population = Random.Range(50,50);
        } 
    }
}
