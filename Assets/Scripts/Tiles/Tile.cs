using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class Tile
{
    public TileTerrain terrain;
    public Nation owner;
    public Nation occupier;
    public int population;
    public Vector3Int tilePos;
    public bool border;
    public bool frontier;
    public bool nationalBorder;
    public bool coastal;

    public float baseGrowthRate = 0.00002f;
    public List<Nation> borderingNations = new List<Nation>();

    public void growPopulation(){
        if (population > 0 && population < 10000 * terrain.biome.fertility){
            int totalGrowth = Mathf.RoundToInt(population * baseGrowthRate);
            if (Random.Range(0f,1f) < (population * baseGrowthRate) - Mathf.FloorToInt(population * baseGrowthRate)){
                totalGrowth += 1;
            }
            population += totalGrowth;
            if (owner){
                owner.population += totalGrowth;
            }
        }
        
    }
}
