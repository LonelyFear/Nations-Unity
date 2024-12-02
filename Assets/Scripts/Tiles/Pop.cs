using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pop
{
    // Population
    public int population;
    public int dependents;
    public int workforce;
    public float dependentRatio = 0.75f;

    const float baseDependentRatio = 0.75f;
    const float baseBirthRate = 0.04f;
    const float baseDeathRate = 0.037f;

    // Stats
    public Tile tile;
    public State state;
    public Culture culture;

    enum PopStates {
        HUNTER_GATHERER,
        SETTLED
    }

    PopStates status = PopStates.HUNTER_GATHERER;

    public void Tick(){
        if (tile == null){
            DeletePop();
        }

        if (tile != null){
            AssimilateCulture();
            MergePops();  
        }
        // Top functions can unnasign pop from tile
        if (population > 0 && tile != null){


            dependentRatio = Mathf.Lerp(dependentRatio, baseDependentRatio, 0.03f);
            CalcDependents();

            state = tile.state;
            GrowPopulation();
            
            if (status == PopStates.HUNTER_GATHERER){
                if (state != null && Random.Range(0f, 1f) < 0.25f){
                    status = PopStates.SETTLED;
                } else {
                    HGMigration();
                }
            } else {
                SettlerMigration();
            }
        }
    }
    void SettlerMigration(){
        if (Random.Range(0f, 1f) < 0.01f && population > 100){
            foreach (Tile target in tile.borderingTiles){
                // If the target has significantly lower population
                bool tileNotFull = target.population * 4 < tile.population && Random.Range(0f, 1f) < 0.2f * target.terrain.navigability;
                // If the tile is close to its maximum population
                bool tileFull = tile.population * 1.2 > tile.GetMaxPopulation() && Random.Range(0f, 1f) < 0.5f * target.terrain.navigability;
                if (tileNotFull || tileFull){
                    MoveTile(target, Mathf.RoundToInt(population * Random.Range(0.2f, 0.5f)));
                }
            }
        } 
    }
    void HGMigration(){
        // 0.005 is the chance a hunter gatherer moves
        float moveChance = 0.05f;
        if (Random.Range(0f, 1f) < moveChance && population > 100){
            foreach (Tile target in tile.borderingTiles){
                bool coastal = tile.coastal && Random.Range(0f, 1f) <= target.terrain.navigability;
                bool inland = Random.Range(0f, 1f) <= 0.05 * target.terrain.navigability;
                if (target.population * 2 < tile.population && (inland || coastal)){
                    MoveTile(target, Mathf.RoundToInt(population * Random.Range(0.2f, 0.5f)));
                }
            }
        }    
    }

    public void GrowPopulation(){
        float birthRate = 0f;

        if (population > 1){
            birthRate = baseBirthRate;
            if (tile.population < 500){
                birthRate += 0.01f;
            }
            if (population > tile.GetMaxPopulation()){
                birthRate *= 0.75f;
            }
        }

        float deathRate = baseDeathRate;
        float natutalGrowthRate = birthRate - deathRate;

        int totalGrowth = Mathf.RoundToInt(population * natutalGrowthRate);

        if (Random.Range(0f,1f) < (population * Mathf.Abs(natutalGrowthRate)) - Mathf.FloorToInt(population * Mathf.Abs(natutalGrowthRate))){
            totalGrowth += (int) Mathf.Sign(natutalGrowthRate);
        }
        if (totalGrowth != 0){
            ChangePopulation(totalGrowth);
        }
    }

    public void MoveTile(Tile newTile, int amount){
        // Checks if we are actually moving people to a new tile
        if (newTile != tile && amount > 0 && !newTile.terrain.water){
            if (amount > population){
                // If the amount we are trying to move is greater than our population
                // Sets it to our population to prevent creating people out of nothing
                amount = population;
            }
            Pop popToMerge = null;
            foreach (Pop pop in newTile.pops){
                // Goes through all the pops in our target tile
                if (pop.culture == culture && pop.status == status){
                    // If the pop matches up with our pop
                    // Sets our pop to merge with that pop
                    popToMerge = pop;
                    break;
                }
            }
            if (popToMerge != null){
                // Changes the pop we are merging with by amount
                popToMerge.ChangePopulation(amount);
            } else {
                // Otherwise makes a new pop with our culture
                Pop newPop = new Pop{
                    population = amount,
                    culture = culture,
                    status = status
                };
                // And moves that new pop into the tile
                newPop.SetTile(newTile);
                TimeEvents.tick += newPop.Tick;
            }
            // Finally subtracts the amount from our population
            ChangePopulation(amount * -1);
        }
       
    }

    public void SetTile(Tile newTile){
        if (tile != null){
            tile.pops.Remove(this);
            tile.population -= population;
            tile.workforce -= workforce;
        }

        tile = newTile;

        if (newTile != null){
            newTile.population += population;
            newTile.workforce += workforce;
            newTile.pops.Add(this);       

            SetState(newTile.state);
        }  
    }

    public void SetState(State newState){
        if (state != null){
            //state.pops.Remove(this);
            state.population -= population;
            state.totalPopulation -= population;
            state.workforce -= workforce;
        }

        state = newState;

        if (newState != null){
            newState.population += population;
            newState.totalPopulation += population;
            newState.workforce += workforce;
            /*
            if (!state.pops.Contains(this)){
                state.pops.Add(this);        
            }
            */
        }  
    }

    public void SetPopulation(int amount){
        if (tile != null){
            tile.population -= population;
            if (state != null){
                state.population -= population;
                state.totalPopulation -= population;
            }
            CalcDependents();
        }

        population = amount;

        if (tile != null){
            tile.population += population;
            if (state != null){
                state.population += population;
                state.totalPopulation += population;
            }
        }
    }
    public void ChangePopulation(int amount){
        int totalChange = amount;

        // Changes population
        if (population + amount < 1){
            DeletePop();
            CalcDependents();
        } else {
            population += amount;
            CalcDependents();
        }

        // Updates statistics
        if (tile != null){
            tile.ChangePopulation(totalChange);
        }
    }

    public void ChangeWorkforce(int amount){
        CalcDependents();
        int totalChange = amount;
        if (workforce + amount < 1){
            totalChange = workforce * -1;
        }
        workforce += totalChange;
        dependentRatio = dependents + 0.001f / (dependents + workforce + 0.001f);

        CalcDependents();
        ChangePopulation(totalChange);
    }

    public void CalcDependents(){
        if (tile != null){
            tile.ChangeWorkforce(-1 * workforce);

            dependents = Mathf.RoundToInt(population * dependentRatio);
            workforce = population - dependents;

            tile.ChangeWorkforce(workforce);
        }
        
    }

    public void DeletePop(){
        if (tile != null){
            tile.pops.Remove(this);
            tile.ChangePopulation(-population);
            // Adjusts the workforces of our tile and state
            tile.ChangePopulation(-workforce);
        }
        population = 0;
        tile = null;
        state = null;
    }

    void AssimilateCulture(){
        foreach (Pop pop in tile.pops.ToArray()){
            if (pop == this){
                continue;
            }
            Color cultureColor = culture.color;
            Color otherCulture = pop.culture.color;
            float minDifference = 0.2f;
            bool similarRed = Math.Abs(cultureColor.r - otherCulture.r) <=minDifference;
            bool similarGreen = Math.Abs(cultureColor.g - otherCulture.g) <=minDifference;
            bool similarBlue = Math.Abs(cultureColor.b - otherCulture.b) <=minDifference;
            if (similarRed && similarGreen && similarBlue){
                culture = pop.culture;
            }
        }
    }

    void MergePops(){
        foreach (Pop pop in tile.pops.ToArray()){
            if (pop == this){
                continue;
            }
            if (pop.culture == culture && pop.status == status && pop.tile == tile){
                ChangePopulation(pop.population);
                pop.DeletePop();
            }
        }
    }
}
