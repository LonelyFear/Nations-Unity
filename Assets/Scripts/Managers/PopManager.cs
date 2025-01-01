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

public class PopManager : MonoBehaviour
{   
    public List<Pop> pops = new List<Pop>();
    public int worldPopulation;
    int currentIndex;

    [SerializeField] TileManager tm;

    public void Awake(){
        //populations = new int[99999];
        TimeEvents.tick += Tick;
    }

    public void Tick(){
        GrowPopulations();

        for (int i = 0; i < pops.Count; i++){
            Pop pop = pops[i];

            if (pop.population <= 0){
                DeletePop(pop);
                continue;
            }
            // Sets workforce and dependents
            pop.workforce = Mathf.RoundToInt(pop.population * pop.workforceRatio);
            pop.dependents = pop.population - pop.workforce;
            
            //MovePop(pops.IndexOf(pop), tm.getTile(new Vector3Int(pop.tile.tilePos.x - 1 , pop.tile.tilePos.y)), Mathf.RoundToInt(pop.population * 0.5f));
            SimilarPops(i, Random.Range(0, pops.Count - 1));
        }
    }

    void GrowPopulations(){
        int[] populations = new int[pops.Count];
        for (int i = 0; i < pops.Count; i++){
            populations[i] = pops[i].population;
        }
        NativeArray<int> output = new NativeArray<int>(pops.Count, Allocator.TempJob);
        NativeArray<int> population = new NativeArray<int>(populations.ToArray(), Allocator.TempJob);

        GrowPopJob popJob = new GrowPopJob(){
            population = population,
            outputs = output,
            seed = (uint)UnityEngine.Random.Range(1, 1000)
        };

        JobHandle jobHandle = popJob.Schedule(pops.Count, 32);
        jobHandle.Complete();

        for (int i = 0; i < pops.Count; i++){
            ChangePopulation(i, output[i]);
        }

        output.Dispose();
        population.Dispose();
    }

    public void CreatePop(int population, Culture culture, Tile tile = null, Tech tech = null){
        if (population > 0){
            Pop pop = new Pop();

            // Updates Lists
            pops.Add(pop);

            pop.index = currentIndex;
            currentIndex++;

            worldPopulation += population;

            pop.population = population;
            pop.culture = culture;
            pop.popManager = this;

            
            pop.dependents = 1;

            //TimeEvents.tick += pop.Tick;

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

    public void MovePop(int popIndex, Tile newTile, int amount){
        
        bool moved = false;
        if (newTile != null && !newTile.terrain.water){
            if (amount > pops[popIndex].population){
                amount = pops[popIndex].population;
            }
            foreach (Pop merger in newTile.pops){
                if (SimilarPops(popIndex, pops.IndexOf(merger))){
                    ChangePopulation(pops.IndexOf(merger), amount);
                    moved = true;
                    break;
                }
            }
            if (!moved){
                CreatePop(amount, pops[popIndex].culture, newTile, pops[popIndex].tech);
            }
            //print("Called");
            ChangePopulation(pops.IndexOf(pops[popIndex]), -amount);
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
            ChangePopulation(pops.IndexOf(b), a.population);
            ChangePopulation(pops.IndexOf(a), -a.population);
            merged = true;
        }

        return merged;
    }
    void EarlyMigration(int index){
        float moveChance = 0.2f;
        if (UnityEngine.Random.Range(0f, 1f) < moveChance && pops[index].tile != null){
            foreach (Tile target in pops[index].tile.borderingTiles.ToArray()){
                bool coastal = pops[index].tile.coastal && UnityEngine.Random.Range(0f, 1f) <= target.terrain.fertility;
                bool inland = UnityEngine.Random.Range(0f, 1f) <= 0.05 * target.terrain.fertility;
                if (target.population * 4 < pops[index].tile.population && (inland || coastal)){
                    MovePop(index, target, Mathf.RoundToInt(pops[index].population * UnityEngine.Random.Range(0.2f, 0.5f)));
                }
            }
        } 
    }
    public void ChangePopulation(int index, int amount){
        int population = pops[index].population;
        int totalChange = amount;

        // Changes population
        
        if (population + amount < 1){
            totalChange = -population;
        }

        pops[index].population += totalChange;
        if (pops[index].tile != null){
            pops[index].tile.ChangePopulation(totalChange);
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

    [BurstCompile]
    public struct GrowPopJob : IJobParallelFor
    {
        public NativeArray<int> population;

        public NativeArray<int> outputs;
        public uint seed;
        public void Execute(int i)
        {
            float birthRate = 0.4f;
            float deathRate = 0.37f;            
            var rand = new Unity.Mathematics.Random((uint)(seed + i * 40));

            if (population[i] < 2){
                birthRate = 0f;
            }
            if (population[i] > 10000){
                birthRate *= 0.75f;
            }

            float natutalGrowthRate = birthRate - deathRate;
            int totalGrowth = Mathf.RoundToInt(population[i] * natutalGrowthRate);
            
            if (rand.NextDouble() < (population[i] * Math.Abs(natutalGrowthRate)) - Math.Floor(population[i] * Math.Abs(natutalGrowthRate))){
                totalGrowth += (int) Math.Sign(natutalGrowthRate);
            }

            if (totalGrowth != 0){
                population[i] += totalGrowth;
                outputs[i] = totalGrowth;
            }
        }
    }
}

