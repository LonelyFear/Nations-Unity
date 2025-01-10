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

public class PopManager : MonoBehaviour
{   
    public List<Pop> pops = new List<Pop>();
    public int worldPopulation;
    int currentIndex;

    // Constants
    const float baseworkforceRatio = 0.25f;
    const float baseBirthRate = 0.04f;
    const float baseDeathRate = 0.036f;

    [SerializeField] TileManager tm;

    public void Awake(){
        //populations = new int[99999];
        TimeEvents.tick += Tick;
    }

    public enum PopStates {
        MIGRATORY,
        SETTLED
    }

    public void Tick(){
    }

    public void CreatePop(int population, Culture culture, Tile tile = null, Tech tech = null, float workforceRatio = 0.25f){
        if (population > 0){
            Pop pop = new Pop();

            // Updates Lists
            pops.Add(pop);
            TimeEvents.tick += pop.Tick;

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
        }

    }

    bool SimilarPops(int indexA, int indexB){
        Pop a = pops[indexA];
        Pop b = pops[indexB];
        if (a == null || b == null || a == b){
            return false;
        }

        return true;
    }
    bool MergePops(int indexA, int indexB){
        Pop a = pops[indexA];
        Pop b = pops[indexB];
        bool merged = false;
        if (SimilarPops(indexA, indexB)){
            ChangePopulation(a, a.population);
            ChangePopulation(b, -a.population);
            merged = true;
        }

        return merged;
    }

    public void ChangePopulation(Pop pop, int amount, float workforceRatio = 0.25f){
        int population = pop.population;
        int totalChange = amount;

        // Changes population
        
        if (population + amount < 1){
            totalChange = -population;
        }
        int workforceChange = Mathf.FloorToInt((float)population * workforceRatio);
        if (Random.Range(0f, 1f) < Mathf.Abs((float)population * workforceRatio) % 1){
            workforceChange += 1;
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
        if (pop.tile != null){
            pop.tile.ChangeWorkforce(totalChange);
            pop.tile.ChangePopulation(totalChange);
        }
        
        worldPopulation += totalChange;
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

