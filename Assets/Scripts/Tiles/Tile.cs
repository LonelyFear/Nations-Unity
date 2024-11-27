using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class Tile
{
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
    public const int maxPops = 25;
    public const float baseBirthRate = 0.04f;
    public const float baseDeathRate = 0.037f;

    public int GetMaxPopulation(){
        return Mathf.RoundToInt(10000 * terrain.biome.fertility);
    }

    public void GrowPopulation(){
        Pop[] popsArray = pops.ToArray();
        foreach (Pop pop in popsArray){
            pop.GrowPopulation();
        }
    }

    public void ChangePopulation(int amount){
        int totalChange = amount;
        if (population + amount < 1){
            totalChange = population * -1;
            population = 0;

        } else {
            population += amount;
        }
        if (state != null){
            state.population += totalChange;
        }
    }
}
