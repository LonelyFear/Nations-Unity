using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Tilemaps;

public class Pop
{
    // Population
    public int population;
    //int dependents;
    //int workers;
    public const float baseBirthRate = 0.04f;
    public const float baseDeathRate = 0.037f;

    // Stats
    public Tile tile;
    public State state;
    public Culture culture;

    public void GrowPopulation(){
        float birthRate = 0f;

        if (population > 1){
            birthRate = baseBirthRate;
            if (population > tile.GetMaxPopulation()){
                birthRate *= 0.75f;
            }
        }

        float deathRate = baseDeathRate;
        float natutalGrowthRate = (birthRate - deathRate) / 12;

        int totalGrowth = Mathf.RoundToInt(population * natutalGrowthRate);

        if (Random.Range(0f,1f) < (population * Mathf.Abs(natutalGrowthRate)) - Mathf.FloorToInt(population * Mathf.Abs(natutalGrowthRate))){
            totalGrowth += (int) Mathf.Sign(natutalGrowthRate);
        }

        ChangePopulation(totalGrowth);
    }

    public void SetTile(Tile newTile){
        if (tile != null){
            tile.pops.Remove(this);
            tile.population -= population;
        }

        tile = newTile;

        if (tile != null){
            tile.population += population;
            tile.pops.Add(this);       

            SetState(tile.state);
        }  
    }

    public void SetState(State newState){
        if (state != null){
            //state.pops.Remove(this);
            state.population -= population;
        }

        state = newState;

        if (state != null){
            state.population += population;
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
            }
        }

        population = amount;

        if (tile != null){
            tile.population += population;
            if (state != null){
                state.population += population;
            }
        }
    }
    public void ChangePopulation(int amount){
        int totalChange = amount;

        // Changes population
        if (population + amount < 1){
            DeletePop();
        } else {
            population += amount;
        }

        // Updates statistics
        if (tile != null){
            tile.ChangePopulation(totalChange);
        }
        
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

    public static void MergePops(Pop pop1, Pop pop2){
        if (pop1.culture == pop2.culture && pop1.tile == pop2.tile){
            pop1.ChangePopulation(pop2.population);
            pop2.DeletePop();
        }
    }
}
