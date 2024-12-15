using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using System;
using Unity.Collections;
using Unity.Mathematics;

public class PopManager : MonoBehaviour
{   
    [SerializeField] private Pop popPrefab;
    public List<Pop> pops = new List<Pop>();
    List<int> populations = new List<int>();

    public void Awake(){
        TimeEvents.tick += Tick;
    }

    public void Tick(){
        GrowPopulations();
    }

    void GrowPopulations(){
        NativeArray<int> numbers = new NativeArray<int>(pops.Count, Allocator.TempJob);
        NativeArray<int> population = new NativeArray<int>(populations.ToArray(), Allocator.TempJob);

        GrowPopJob popJob = new GrowPopJob(){
            population = population,
            outputs = numbers,
            seed = (uint)UnityEngine.Random.Range(1, 1000)
        };

        JobHandle jobHandle = popJob.Schedule(pops.Count, 64);
        jobHandle.Complete();

        for(int i = 0; i < pops.Count; i++){
            ChangePopPopulation(pops[i], popJob.outputs.ToArray()[i]);
        }

        numbers.Dispose();
        population.Dispose();
    }

    public void CreatePop(int population, Culture culture, Tile tile = null, Tech tech = null){
        Pop pop = Instantiate(popPrefab);

        // Updates Lists
        pops.Add(pop);
        populations.Add(population);

        pop.population = population;
        pop.culture = culture;
        pop.popManager = this;
        pop.transform.parent = transform;
        TimeEvents.tick += pop.Tick;
        if (tech == null){
            pop.tech = new Tech();
        } else {
            pop.tech = tech;
        }
        if (tile != null){
            SetPopTile(pop, tile);
        }
    }

    public void ChangePopPopulation(Pop pop, int amount, bool updateTile = true){
        int totalChange = amount;

        // Changes population
        if (pop.population + amount < 1){
            totalChange = -pop.population;
            pop.popManager.DeletePop(pop);
            pop.CalcDependents();
        } else {
            pop.population += amount;
            pop.CalcDependents();
        }

        // Updates statistics
        if (pop.tile != null && updateTile){
            pop.tile.ChangePopulation(totalChange);
        }
        if (pops.Contains(pop)){
            populations[pops.IndexOf(pop)] = pop.population;
        }
    }

    public void DeletePop(Pop pop){
        pops.Remove(pop);
        if (pop.tile != null){
            pop.tile.pops.Remove(pop);
            pop.tile.ChangePopulation(-pop.population);
            // Adjusts the workforces of our tile and state
            pop.tile.ChangePopulation(-pop.workforce);
        }
        Destroy(pop.gameObject);
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

            float natutalGrowthRate = birthRate - deathRate;
            int totalGrowth = Mathf.RoundToInt(population[i] * natutalGrowthRate);

            if (rand.NextDouble() < (population[i] * Math.Abs(natutalGrowthRate)) - Math.Floor(population[i] * Math.Abs(natutalGrowthRate))){
                totalGrowth += (int) Math.Sign(natutalGrowthRate);
            }

            if (totalGrowth != 0){
                population[i] += totalGrowth;
                outputs[i] = population[i];
            }
        }
    }
}

