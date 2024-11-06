using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public TileTerrain terrain;    public State state = null;
    public int totalPopulation;
    public List<Pop> pops = new List<Pop>();
    public Vector3Int tilePos;
    public bool border;
    public bool frontier;
    public bool nationalBorder;
    public bool coastal = false;
    public bool anarchy = true;

    public int carryingCapacity {get; private set;}

    public List<State> borderingStates = new List<State>();

    public int getMaxPopulation(){
        return Mathf.RoundToInt(10000 * terrain.biome.fertility);
    }

    public void growPopulation(){
        foreach (Pop pop in pops){
            pop.growPopulation();
        }  
    }
}
