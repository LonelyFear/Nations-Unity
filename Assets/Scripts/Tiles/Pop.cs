using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
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
    const float baseDeathRate = 0.036f;

    // Stats
    public Tile tile;
    public State state;
    public Culture culture;
    public Tech tech = new Tech();


    public enum PopStates {
        MIGRATORY,
        SETTLED
    }

    public PopStates status = PopStates.MIGRATORY;
    public void Tick(){
        if (tile == null){
            DeletePop();
        }

        if (tile != null && tile.pops.Count > 1){
            AssimilateCulture();
            MergePops();  
        }

        // Top functions can unnasign pop from tile
        if (population > 0 && tile != null){

            dependentRatio = Mathf.Lerp(dependentRatio, baseDependentRatio, 0.03f);
            CalcDependents();

            state = tile.state;
            GrowPopulation();

            if (status == PopStates.MIGRATORY){
                if (state != null && Random.Range(0f, 1f) < 0.25f){
                    status = PopStates.SETTLED;
                } else {
                    EarlyMigration();
                }
            } else {
                if (tile != null){
                    // Settled functions
                    SimpleMigration();
                    DevelopTech();
                    //SettlerMigration();                    
                }

            }
        }
    }
    void DevelopTech(){
        if (Random.Range(0f, 1f) < 0.0001f + Mathf.Clamp(tile.development / 2f, 0f, 1f)){
            tech.societyLevel += 1;
        }
    }
    void EarlyMigration(){
        float moveChance = 0.2f;
        if (Random.Range(0f, 1f) < moveChance && tile != null){
            foreach (Tile target in tile.borderingTiles.ToArray()){
                bool coastal = tile.coastal && Random.Range(0f, 1f) <= target.terrain.fertility;
                bool inland = Random.Range(0f, 1f) <= 0.05 * target.terrain.fertility;
                if (target.population * 4 < tile.population && (inland || coastal)){
                    MoveTile(target, Mathf.RoundToInt(population * Random.Range(0.2f, 0.5f)));
                }
            }
        }    
    }

    void SimpleMigration(){
        float moveChance = 0.08f;
        if (Random.Range(0f, 1f) < moveChance && tile != null){
            Tile target = tile.borderingTiles[Random.Range(0, tile.borderingTiles.Count - 1)];
            bool fertile = Random.Range(0f, 1f) <= 0.5 * target.terrain.fertility;
            bool developed = Random.Range(0f, 1f) <= 0.5 * target.development;
            bool lowPop = Random.Range(0f, 1f) <= 0.5 / (target.population / tile.population);
            bool noPop = (target.pops.Count == 0 && Random.Range(0f, 1f) <= 0.9) ? true : false;
            bool sameState = (target.state == state || target.state != state && Random.Range(0f, 1f) <= 0.9) ? true : false;

            bool rulingPopCheck = state.rulingPop == this && population > 100 || state.rulingPop != this;

            if ((fertile || developed || lowPop || noPop) && sameState && rulingPopCheck){
                MoveTile(target, Mathf.RoundToInt(population * Random.Range(0.2f, 0.5f)));
            }
        }
    }
    public void GrowPopulation(){
        float birthRate = 0f;
        

        if (population > 1){
            birthRate = baseBirthRate;

            if (population > tile.GetMaxPopulation()){
                birthRate *= 0.75f;
            }
            if (tech.societyLevel > 5){
                birthRate -= Mathf.Clamp((tech.societyLevel - 5)/900, 0f, 0.5f);
            }
        }

        float deathRate = baseDeathRate - Mathf.Clamp(tech.societyLevel/1000, 0f, 0.3f);

        
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
                if (pop.culture == culture && CheckSimilarTech(pop)){
                    // If the pop matches up with our pop
                    // Sets our pop to merge with that pop
                    popToMerge = pop;
                    break;
                }
            }
            if (popToMerge != null){
                // Changes the pop we are merging with by amount
                popToMerge.ChangePopulation(amount);
                //popToMerge.AssimilateCulture();
            } else {
                // Otherwise makes a new pop with our culture
                Pop newPop = new Pop{
                    population = amount,
                    culture = culture,
                    tech = tech,
                };
                // And moves that new pop into the tile
                newPop.SetTile(newTile);
                TimeEvents.tick += newPop.Tick;
                //newPop.AssimilateCulture();
            }
            if (tile.tileManager.mapMode == TileManager.MapModes.CULTURE){
                tile.UpdateColor();
            }
            // Finally subtracts the amount from our population
            ChangePopulation(amount * -1);
        }
       
    }

    public bool CheckSimilarTech(Pop pop){
        float minTechDiff = 0.5f;
        Tech techToCheck = pop.tech;
        if (pop != this){
            bool similarSociety = techToCheck.societyLevel - tech.societyLevel <= minTechDiff;
            bool similarMilitary = techToCheck.militaryLevel - tech.militaryLevel <= minTechDiff;
            bool similarIndustry = techToCheck.industryLevel - tech.industryLevel <= minTechDiff;
            if (similarIndustry && similarMilitary && similarSociety){
                return true;
            }
        }
        return false;
    }

    public void SetTile(Tile newTile){
        if (tile != null){
            tile.pops.Remove(this);
            tile.ChangePopulation(-population);
            tile.ChangeWorkforce(-workforce);
        }

        tile = newTile;

        if (newTile != null){
            newTile.ChangePopulation(population);
            newTile.ChangeWorkforce(workforce);
            newTile.pops.Add(this);       

            //SetState(newTile.state);
        }  
    }

    /*
    public void SetState(State newState){
        if (state != null){
            state.population -= population;
            state.totalPopulation -= population;
            state.workforce -= workforce;
        }

        state = newState;

        if (newState != null){
            newState.population += population;
            newState.totalPopulation += population;
            newState.workforce += workforce;
        }  
    }
    */
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
    public void ChangePopulation(int amount, bool updateTile = true){
        int totalChange = amount;

        // Changes population
        if (population + amount < 1){
            totalChange = -population;
            DeletePop();
            CalcDependents();
        } else {
            population += amount;
            CalcDependents();
        }

        // Updates statistics
        if (tile != null && updateTile){
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
            if (tile.tileManager.mapMode == TileManager.MapModes.CULTURE){
                tile.UpdateColor();
            }
        }
    }

    void MergePops(){
        foreach (Pop pop in tile.pops.ToArray()){
            if (pop == this){
                continue;
            }
            if (pop.culture == culture && pop.tile == tile && CheckSimilarTech(pop)){
                ChangePopulation(pop.population);
                pop.DeletePop();
            }
        }
        if (tile.tileManager.mapMode == TileManager.MapModes.CULTURE){
            tile.UpdateColor();
        }
    }
}
