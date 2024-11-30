using UnityEngine;
using Random = UnityEngine.Random;

public class Pop
{
    // Population
    public int population;
    public int dependents;
    public int workforce;
    public float dependentRatio = 0.75f;
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

    PopStates popState = PopStates.HUNTER_GATHERER;

    public void Tick(){
        if (tile == null){
            DeletePop();
        }

        if (population > 0 && tile != null){
            state = tile.state;
            GrowPopulation();
            
            if (popState == PopStates.HUNTER_GATHERER){
                if (state != null && Random.Range(0f, 1f) < 0.25f){
                    popState = PopStates.SETTLED;
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
                bool tileNotFull = target.population * 4 < tile.population && Random.Range(0f, 1f) < 0.05f * target.terrain.fertility;
                // If the tile is close to its maximum population
                bool tileFull = tile.population * 1.2 > tile.GetMaxPopulation() && Random.Range(0f, 1f) < 0.2f * target.terrain.fertility;
                if (tileNotFull || tileFull){
                    MoveTile(target, Mathf.RoundToInt(population * Random.Range(0.2f, 0.5f)));
                }
            }
        } 
    }
    void HGMigration(){
        // 0.005 is the chance a hunter gatherer moves
        float moveChance = 0.005f;
        if (Random.Range(0f, 1f) < moveChance && population > 100){
            foreach (Tile target in tile.borderingTiles){
                if (target.population * 2 < tile.population && Random.Range(0f, 1f) <= 1 * target.terrain.fertility){
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
                if (pop.culture == culture && pop.popState == popState){
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
                    popState = popState
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
        }

        tile = newTile;

        if (newTile != null){
            newTile.population += population;
            newTile.pops.Add(this);       

            SetState(newTile.state);
        }  
    }

    public void SetState(State newState){
        if (state != null){
            //state.pops.Remove(this);
            state.population -= population;
            state.totalPopulation -= population;
        }

        state = newState;

        if (state != null){
            state.population += population;
            state.totalPopulation += population;
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

    public void CalcDependents(){
        dependents = Mathf.RoundToInt(population * dependentRatio);
        workforce = population - dependents;
    }

    public void DeletePop(){
        if (tile != null){
            tile.pops.Remove(this);
            tile.ChangePopulation(-population);
        }
        population = 0;
        tile = null;
        state = null;
    }
}
