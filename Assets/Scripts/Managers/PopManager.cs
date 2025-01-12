using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using System;
using Unity.Collections;

using Random = UnityEngine.Random;
using UnityEngine.Jobs;
using UnityEngine.Rendering.Universal;


public class PopManager : MonoBehaviour
{  
    public int worldPopulation;
    public int worldWorkforce;
    public float worldRatio;
    int currentIndex;

    // Constants
    const float baseworkforceRatio = 0.25f;
    const float baseBirthRate = 0.04f;
    const float baseDeathRate = 0.036f;

    [SerializeField] TileManager tm;

    NativeList<Pop> pops = new NativeList<Pop>(Allocator.Persistent);

    public void Awake(){
        //populations = new int[99999];
        Events.tick += Tick;
    }

    public enum PopStates {
        MIGRATORY,
        SETTLED
    }

    public void Tick(){
        //NativeList<Pop> nPops = pops.ToNativeList(Allocator.TempJob);
        NativeArray<int> output = new NativeArray<int>(pops.Length, Allocator.TempJob);
        NaturalGrowthJob naturalGrowthJob = new NaturalGrowthJob{
            pops = pops,
            output = output,
            seed = Random.Range(1, 10000)
        };
       
        JobHandle handle0 = naturalGrowthJob.Schedule(pops.Length, 1000);
        handle0.Complete();

        foreach (int o in output){
            worldPopulation += o;
        }
        output.Dispose();
    }

    public Pop CreatePop(int population, Culture culture, TileStruct tile, Tech tech = new Tech(), float workforceRatio = 0.25f){
        Pop pop = new Pop(){
            population = population,
            dependents = population - Mathf.RoundToInt((float)population * workforceRatio),
            workforce = Mathf.RoundToInt((float)population * workforceRatio),
            birthRate = 0.04f / TimeManager.ticksPerYear,
            deathRate = 0.036f / TimeManager.ticksPerYear,
            index = currentIndex,
            tech = tech,
            tile = tile
        };
        tile.population += population;

        // Updates Lists
        pops.Add(pop);
        //Events.popTick += pop.Tick;
        currentIndex++;

        worldPopulation += population;

        pop.culture = culture;
        return pop;
    }

    [BurstCompile]
    struct NaturalGrowthJob : IJobParallelFor
    {
        [ReadOnly] public NativeList<Pop> pops;
        public NativeArray<int> output;
        [ReadOnly] public int seed;
        public void Execute(int i)
        {
            Pop pop = pops[i];

            float birthRate = pop.birthRate;
            float deathRate = pop.deathRate;            
            var rand = new Unity.Mathematics.Random((uint)(seed + i * 40));

            if (pop.population < 2){
                birthRate = 0f;
            }
            if (pop.population > pop.tile.maxPopulation){
                birthRate *= 0.75f;
            }

            float natutalGrowthRate = birthRate - deathRate;
            int totalGrowth = (int)Math.Floor(pop.population * natutalGrowthRate);
            
            if (rand.NextDouble() < pop.population * Math.Abs(natutalGrowthRate) % 1){
                totalGrowth += (int)Math.Sign(natutalGrowthRate);
            }

            if (totalGrowth != 0){
                pop.population += totalGrowth;
                pop.tile.population += totalGrowth;
                output[i] = totalGrowth;
            }
        }
    }

}

