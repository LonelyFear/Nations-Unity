using System.Collections.Generic;
using UnityEngine;

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
    public const float baseBirthRate = 0.04f;
    public const float baseDeathRate = 0.039f;

    public int getMaxPopulation(){
        return Mathf.RoundToInt(1000 * terrain.biome.fertility);
    }

    public void growPopulation(){
        float birthRate = 0f;
        if (population > 1){
            birthRate = baseBirthRate;
            if (population > getMaxPopulation()){
                birthRate *= 0.75f;
            }
        }
        // -5.5 - -6
        float deathRate = baseDeathRate;
        float natutalGrowthRate = (birthRate - deathRate) / 12;

        int totalGrowth = Mathf.RoundToInt(population * natutalGrowthRate);
        if (Random.Range(0f,1f) < Mathf.Abs((population * natutalGrowthRate) - (Mathf.Sign(natutalGrowthRate) * Mathf.FloorToInt(population * natutalGrowthRate)))){
            totalGrowth += (int) Mathf.Sign(natutalGrowthRate);
        }

        changePopulation(totalGrowth);
    }

    public void changePopulation(int amount){
        int totalChange = amount;
        if (population + amount < 1){
            totalChange = -population;
            population = 0;

        } else {
            population += amount;
        }
        if (state != null){
            state.population += totalChange;
        }
    }
}
