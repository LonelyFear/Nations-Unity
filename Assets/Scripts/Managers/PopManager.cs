using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using System;
using Unity.Collections;
using Unity.VisualScripting;
using System.Linq;
using TMPro;

public class PopManager : MonoBehaviour
{   
    public List<Pop> pops = new List<Pop>();
    public List<int> populations = new List<int>();
    public int worldPopulation;

    [SerializeField] TileManager tm;

    public void Awake(){
        TimeEvents.tick += Tick;
    }

    public void Tick(){
        GrowPopulations();

        for (int i = 0; i < pops.Count; i++){
            Pop pop = pops[i];
            pop.index = pops.IndexOf(pop);
            pop.population = populations[pop.index];
            if (pop.population <= 0){
                DeletePop(pop);
                continue;
            }
            MovePop(pop, tm.getTile(new Vector3Int(pop.tile.tilePos.x - 1 , pop.tile.tilePos.y)), Mathf.RoundToInt(pop.population * 0.5f));
        }
    }

    void GrowPopulations(){
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
            populations.Add(population);
            pop.index = pops.IndexOf(pop);

            worldPopulation += population;

            pop.population = populations[pop.index];
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

    public void MovePop(Pop pop, Tile newTile, int amount){
        
        bool moved = false;
        if (newTile != null && !newTile.terrain.water){
            if (amount > pop.population){
                amount = pop.population;
            }
            foreach (Pop merger in newTile.pops){
                if (SimilarPops(pop, merger)){
                    ChangePopulation(pops.IndexOf(merger), amount);
                    moved = true;
                    break;
                }
            }
            if (!moved){
                CreatePop(amount, pop.culture, newTile, pop.tech);
            }
            //print("Called");
            ChangePopulation(pops.IndexOf(pop), -amount);
        }

    }

    bool SimilarPops(Pop a, Pop b){
        return Culture.CheckSimilarity(a.culture, b.culture) && Tech.CheckSimilarity(a.tech, b.tech) && a.status == b.status;
    }
    bool MergePops(Pop a, Pop b){
        bool merged = false;
        if (SimilarPops(a, b)){
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
                    MovePop(pops[index], target, Mathf.RoundToInt(pops[index].population * UnityEngine.Random.Range(0.2f, 0.5f)));
                }
            }
        } 
    }
    public void ChangePopulation(int index, int amount){
        int population = populations[index];
        int totalChange = amount;

        // Changes population
        if (population + amount < 1){
            totalChange = -population;
        }

        populations[index] += totalChange;
        pops[index].population = populations[index];
        
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
        populations.Remove(pops.IndexOf(pop));
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

