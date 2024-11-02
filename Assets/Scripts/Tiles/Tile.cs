using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public TileTerrain terrain;
    public Nation owner;

    public int totalPopulation;
    public List<Pop> pops = new List<Pop>();
    public Vector3Int tilePos;
    public bool border;
    public bool frontier;
    public bool nationalBorder;
    public bool coastal;

    public List<Nation> borderingNations = new List<Nation>();

    public void growPopulation(){
        foreach (Pop pop in pops){
            pop.growPopulation();
        }  
    }
}
