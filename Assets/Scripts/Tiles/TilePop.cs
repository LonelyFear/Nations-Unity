using Unity.VisualScripting;
using UnityEngine;

public class TilePop : MonoBehaviour
{
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
