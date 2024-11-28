using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class Tile
{
    public TileManager tileManager;
    public TileTerrain terrain;    public State state = null;
    public Vector3Int tilePos;
    public bool border;
    public bool frontier;
    public bool nationalBorder;
    public bool coastal = false;
    public bool anarchy = false;
    public int carryingCapacity {get; private set;}
    public List<State> borderingStates = new List<State>();

    // Population
    public int population;
    public List<Pop> pops = new List<Pop>();
    public const int maxPops = 50;

    public void Tick(){

    }
    public int GetMaxPopulation(){
        // 10k times the fertility is the maximum population a tile can support
        return Mathf.RoundToInt(10000 * terrain.biome.fertility);
    }

    public void ChangePopulation(int amount){
        // Sets our total change to the amount
        int totalChange = amount;
        if (population + amount < 1){
            // If the population plus the amount goes below 1
            // Sets the change to negative population
            totalChange = population * -1;
            population = 0;

        } else {
            // Changes population by amount
            population += amount;
        }
        if (state != null){
            // Updates our state
            state.ChangePopulation(totalChange);
        }
    }
}
