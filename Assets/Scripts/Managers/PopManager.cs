using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using System;
using Unity.Collections;

using Random = UnityEngine.Random;
using UnityEngine.Jobs;
using UnityEngine.Rendering.Universal;
using UnityEditor.U2D.Aseprite;
using System.Collections.Generic;


public class PopManager : MonoBehaviour
{  
    [SerializeField] public static int worldPopulation;
    public static int worldWorkforce;
    public float worldRatio;
    static int currentIndex;

    // Constants
    const float baseworkforceRatio = 0.25f;
    const float baseBirthRate = 0.04f;
    const float baseDeathRate = 0.036f;

    [SerializeField] TileManager tm;

    static NativeList<Pop> pops = new NativeList<Pop>(Allocator.Persistent);

    public void Awake(){
        //populations = new int[99999];
        Events.tick += Tick;
    }

    public enum PopStates {
        MIGRATORY,
        SETTLED
    }

    public void Tick(){
        GrowPopulations();
        Migration();
    }

    void Migration(){
        MigrationJob migrationJob = new MigrationJob{
            pops = pops,
            seed = Random.Range(1, 10000)
        };
        JobHandle handle0 = migrationJob.Schedule(pops.Length, 1000);
        handle0.Complete();
    }

    void GrowPopulations(){
        NaturalGrowthJob naturalGrowthJob = new NaturalGrowthJob{
            pops = pops,
            seed = Random.Range(1, 10000)
        };
       
        JobHandle handle0 = naturalGrowthJob.Schedule(pops.Length, 1000);
        handle0.Complete();      
    }

    public static void ChangePopulation(Pop pop, int amount){
        pop.population += amount;
        pop.tile.ChangePopulation(amount);
    }

    public static Pop CreatePop(int population, Culture culture, Tile tile, Tech tech = new Tech(), float workforceRatio = 0.25f){
        Pop pop = new Pop(){
            population = population,
            dependents = population - Mathf.RoundToInt((float)population * workforceRatio),
            workforce = Mathf.RoundToInt((float)population * workforceRatio),
            birthRate = 0.04f / TimeManager.ticksPerYear,
            deathRate = 0.036f / TimeManager.ticksPerYear,
            index = currentIndex,
            tech = new Tech(){
                industryLevel = tech.industryLevel,
                societyLevel = tech.societyLevel,
                militaryLevel = tech.militaryLevel
            },
            tile = tile.tileData
        };
        tile.tileData.ChangePopulation(population);
        tile.pops.Add(pop);

        // Updates Lists
        pops.Add(pop);
        //Events.popTick += pop.Tick;
        currentIndex++;

        worldPopulation += population;

        pop.culture = culture;
        return pop;
    }

    public static void MovePop(int amount, Pop pop, TileStruct tile){
        bool moved = false;
        if (amount < 1){
            return;
        }
        if (amount > pop.population){
            amount = pop.population;
        }
        foreach (Pop pop2 in tile.pops){
            if (Pop.SimilarPops(pop2, pop)){
                ChangePopulation(pop2, amount);
                moved = true;
                break;
            }
        }
        if (!moved){
            // If we cant merge two pops
            currentIndex++;
            Pop newPop = new Pop(){
                population = amount,
                // dependents = amount - Mathf.RoundToInt((float)population * workforceRatio),
                // workforce = Mathf.RoundToInt((float)population * workforceRatio),
                birthRate = 0.04f / TimeManager.ticksPerYear,
                deathRate = 0.036f / TimeManager.ticksPerYear,
                index = currentIndex,
                tech = new Tech(){
                    industryLevel = pop.tech.industryLevel,
                    societyLevel = pop.tech.societyLevel,
                    militaryLevel = pop.tech.militaryLevel
                },
                tile = tile
            };
            tile.ChangePopulation(amount);
            tile.pops.Add(newPop);            
        }
        ChangePopulation(pop, -amount);
    }

    struct NaturalGrowthJob : IJobParallelFor
    {
        [ReadOnly] public NativeList<Pop> pops;
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
                pop.tile.changeAmount += totalGrowth;
                worldPopulation += totalGrowth;
            }
        }
    }

    struct MigrationJob : IJobParallelFor{
        [ReadOnly] public NativeList<Pop> pops;
        [ReadOnly] public int seed;
        public void Execute(int i)
        {
            Pop pop = pops[i];
            TileStruct tile = pop.tile;
            var rand = new Unity.Mathematics.Random((uint)(seed + i * 40));

            // Checks if we will move this turn
            double moveChance = 0.1f;
            if (pop.status == PopStates.SETTLED){
                moveChance = 0.05f;
            }

            // Checks if the pop will move
            if (moveChance < rand.NextFloat(0f, 1f)){
                // Scores bordering tiles
                NativeHashMap<float, TileStruct> scores = new NativeHashMap<float, TileStruct>(8, Allocator.TempJob);

                foreach (TileStruct target in tile.borderingData){
                    Terrain terrain = target.terrain;
                    float score = 0;
                    if (!terrain.claimable){
                        score = -1000 - rand.NextFloat(6f);
                    } else {
                        score += 10f * terrain.fertility;
                        switch (pop.status){
                            case PopStates.MIGRATORY:
                                score += tile.development;
                                break;
                            case PopStates.SETTLED:
                                score += 2f * tile.development;
                                break;
                        }
                        score += rand.NextFloat(0f, 1f);
                    }
                    scores.Add(score, target);
                }

                float highestScore = -1000;
                TileStruct highestTile = new TileStruct();
                foreach (var entry in scores){
                    if (entry.Key > highestScore){
                        highestTile = entry.Value;
                        highestScore = entry.Key;
                    }
                }
                scores.Dispose();

                if (highestScore > 0){
                    MovePop((int)Math.Round(pop.population * rand.NextFloat(0.1f, 0.5f)), pop, highestTile);
                }
            }
        }
    }

}

