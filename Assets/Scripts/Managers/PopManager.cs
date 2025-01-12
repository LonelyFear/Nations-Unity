using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using System;
using Unity.Collections;
using Unity.VisualScripting;
using System.Linq;
using TMPro;
using System.Data.Common;
using UnityEditorInternal;

using Random = UnityEngine.Random;
using UnityEngine.Jobs;
using System.ComponentModel.Design.Serialization;
using System.Runtime.InteropServices;
using Unity.Entities.UniversalDelegates;

public class PopManager : MonoBehaviour
{   
    public List<Pop> pops = new List<Pop>();
    public int worldPopulation;
    public int worldWorkforce;
    public float worldRatio;
    int currentIndex;

    // Constants
    const float baseworkforceRatio = 0.25f;
    const float baseBirthRate = 0.04f;
    const float baseDeathRate = 0.036f;

    [SerializeField] TileManager tm;

    public void Awake(){
        //populations = new int[99999];
        //Events.tick += Tick;
    }

    public enum PopStates {
        MIGRATORY,
        SETTLED
    }

    public void Tick(){
        //print(pops.Count);
    }

    void GrowPop(Pop pop){
        float bRate = pop.birthRate/TimeManager.ticksPerYear;
        float dRate = pop.deathRate/TimeManager.ticksPerYear;
        if (pop.population < 2){
            bRate = 0f;
        }
        if (pop.tile.population > 10000){
            bRate *= 0.75f;
        }
        float natutalGrowthRate = bRate - dRate;
        int totalGrowth = Mathf.RoundToInt(pop.population * natutalGrowthRate);
        
        if (Random.Range(0f, 1f) < Mathf.Abs(pop.population * natutalGrowthRate) % 1){
            totalGrowth += (int)Mathf.Sign(totalGrowth);
        }

        ChangePopulation(pop, totalGrowth);
    }

    public Pop CreatePop(int population, Culture culture, Tile tile = null, Tech tech = null, float workforceRatio = 0.25f){
        if (population > 0){
            Pop pop = new Pop();

            // Updates Lists
            pops.Add(pop);
            Events.popTick += pop.Tick;

            pop.index = currentIndex;
            currentIndex++;

            worldPopulation += population;

            pop.population = population;
            pop.workforce = Mathf.RoundToInt((float)population * workforceRatio);
            pop.dependents = pop.population - pop.workforce;

            pop.culture = culture;
            pop.popManager = this;

            if (tech == null){
                pop.tech = new Tech();
            } else {
                pop.tech = tech;
            }
            if (tile != null){
                SetPopTile(pop, tile);
            }
            return pop;
        }
        return null;
    }

    bool SimilarPops(int indexA, int indexB){
        Pop a = pops[indexA];
        Pop b = pops[indexB];
        if (Tech.CheckSimilarity(a.tech, b.tech) && Culture.CheckSimilarity(a.culture, b.culture) && a.tile == b.tile){
            return false;
        }

        return true;
    }

    public void ChangePopulation(Pop pop, int amount, float workforceRatio = 0.25f){
        int population = pop.population;
        int totalChange = amount;

        // Changes population
        
        if (population + amount < 1){
            totalChange = -population;
        }

        int workforceChange = Mathf.FloorToInt(totalChange * workforceRatio);
        if (Random.Range(0f, 1f) < Mathf.Abs(totalChange * workforceRatio) % 1){
            workforceChange += (int)Mathf.Sign(totalChange);
        }

        pop.population += totalChange - workforceChange;
        if (pop.tile != null){
            pop.tile.ChangePopulation(totalChange - workforceChange);
        }
        
        worldPopulation += totalChange - workforceChange;

        if (workforceChange > 0){
            ChangeWorkforce(pop, workforceChange);
        }
        
    }
    public void ChangeWorkforce(Pop pop, int amount){
        int workforce = pop.workforce;
        int totalChange = amount;

        // Changes population
        
        if (workforce + amount < 1){
            totalChange = -workforce;
        }

        pop.population += totalChange;
        pop.workforce += totalChange;
        if (pop.tile != null){
            pop.tile.ChangeWorkforce(totalChange);
            pop.tile.ChangePopulation(totalChange);
        }
        
        worldPopulation += totalChange;
        worldWorkforce += totalChange;
    }

    public void DeletePop(Pop pop){
        if (pop.tile != null){
            pop.tile.pops.Remove(pop);
            pop.tile.ChangePopulation(-pop.population);
            // Adjusts the workforces of our tile and state
            pop.tile.ChangeWorkforce(-pop.workforce);
        }

        pops.Remove(pop);
    }

    public void SetPopTile(Pop pop, Tile tile){
        if (pop.tile != null){
            pop.tile.pops.Remove(pop);
            pop.tile.ChangePopulation(-pop.population);
            pop.tile.ChangeWorkforce(-pop.workforce);
        }

        pop.tile = tile;

        if (tile != null){
            tile.ChangePopulation(pop.population);
            tile.ChangeWorkforce(pop.workforce);
            tile.pops.Add(pop);       

            //SetState(newTile.state);
        }  
    }
}

