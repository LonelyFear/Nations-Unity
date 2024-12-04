using System;
using System.Collections.Generic;
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


    enum PopStates {
        MIGRATORY,
        SETTLED
    }

    PopStates status = PopStates.MIGRATORY;
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
                SimpleMigration();
                //SettlerMigration();
            }
            //SettlerMigration();
        }
    }
        void EarlyMigration(){
        float moveChance = 0.2f;
        if (Random.Range(0f, 1f) < moveChance && population > 100){
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
        if (Random.Range(0f, 1f) < moveChance && population > 100){
            Tile target = tile.borderingTiles[Random.Range(0, tile.borderingTiles.Count - 1)];
            float chooseChance = 0.5f;
            chooseChance *= target.terrain.fertility;
            chooseChance *= 1f + target.development;

            if (chooseChance > 0.5f){
                chooseChance = 0.5f;
            }
            if (Random.Range(0f, 1f) < chooseChance){
                MoveTile(target, Mathf.RoundToInt(population * Random.Range(0.2f, 0.5f)));
            }
        }
    }
    void SettlerMigration(){
        float moveChance = 0.1f;
        if (Random.Range(0f, 1f) < moveChance && population > 50){
            Dictionary<Tile, float> candidates = new Dictionary<Tile, float>();
            List<float> attractions = new List<float>();
            foreach (Tile target in tile.borderingTiles.ToArray()){
                candidates.Add(target, CalcAttraction(target));
            }
            Tile selectedTile = null;
            foreach (var entry in candidates){
                Tile candidate = entry.Key;
                float attraction = entry.Value;

                
                if ((selectedTile == null && attraction > 0) || (selectedTile != null && attraction > candidates[selectedTile])){
                    selectedTile = candidate;
                }
            }
            
            if (selectedTile != null && selectedTile != tile){
                
                MoveTile(selectedTile, Mathf.RoundToInt(population * Random.Range(0.2f, 0.5f)));
            }
        } 
    }
    float CalcAttraction(Tile target){
        float attraction = 0f;
        if (tile != null){
            if (target.population <= tile.tileManager.minNationPopulation * 2){
                attraction += 1;
            }
            if (target.population * 1.1f >= tile.GetMaxPopulation()){
                attraction += -1;
            }
            if (target.pops.Count == 0){
                attraction += 1;
            }
            if (target.state == null){
                attraction += 1;
            }
            if (target.population < tile.population && target.population > 0){
                attraction += 1 * ((tile.population + 0.001f) / target.population + 0.001f);
            }
            attraction += (target.development - tile.development) * 1f;

            if (target.state != state && target.state != null){
                attraction += -0.5f;
            }
            attraction += target.terrain.fertility;
        }

        return attraction;
    }

    public void GrowPopulation(){
        float birthRate = 0f;
        

        if (population > 1){
            birthRate = baseBirthRate;

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
                if (pop.culture == culture){
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
            if (pop.culture == culture && pop.tile == tile){
                ChangePopulation(pop.population);
                pop.DeletePop();
            }
        }
        if (tile.tileManager.mapMode == TileManager.MapModes.CULTURE){
            tile.UpdateColor();
        }
    }
}
