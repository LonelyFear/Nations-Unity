using UnityEngine;
using Random=UnityEngine.Random;

public class Pop
{
    public int population = 0;
    public Tile home = null;
    public State state = null;
    public Culture culture = null;
    public Troop troop = null;
    public float wealth = 0;
    public bool canPopChange = true;

    // Base growth and mortality rates
    const float baseGrowthRate = 0.0002f;
    const float baseMortalityRate = 0.00005f;

    public void migrateToTile(int amount, Tile destination){
        if (population - amount >= 0 && amount > 0){
            changePopulation(amount);
            Pop popToJoin = null;
            foreach (Pop pop in destination.pops){
                if (pop.culture == culture){
                    popToJoin = pop;
                }
            }
            home.pops.Remove(this);
            if (popToJoin != null){
                popToJoin.changePopulation(amount);
            } else {
                Pop newPop = new Pop(){
                    population = 0,
                    home = destination,
                    state = destination.state,
                    culture = culture
                };
                newPop.assignToTile(destination);
            }
        }
    }

    public void growPopulation(){
        if (troop == null && canPopChange){
            float growthRate = baseGrowthRate;
            float mortalityRate = baseMortalityRate;
            if (home.totalPopulation > home.getMaxPopulation()){
                growthRate *= 0.5f - (0.001f * (home.getMaxPopulation() - home.totalPopulation));
            }
           state = home.state;
            int totalGrowth = Mathf.RoundToInt(population * (growthRate - mortalityRate));

            if (Random.Range(0f,1f) < (population * growthRate) - Mathf.FloorToInt(population * growthRate)){
                totalGrowth += 1;
            }
            changePopulation(totalGrowth); 
        }
    }

    public void assignToTroop(Troop unit){
        if (troop != null){
            troop.soldiers -= population;
        }
        troop = unit;
        if (unit != null){
            unit.soldiers += population;
        }
    }

    public void assignToState(State newState){
        if (state != null){
            state.population -= population;
        }
        state = newState;
        if (newState != null){
            newState.population += population;
        }
    }

    public void assignToTile(Tile newTile){
        if (home != null){
            home.totalPopulation -= population;
            home.pops.Remove(this);
        }
        home = newTile;
        if (newTile != null){
            newTile.totalPopulation += population;
            newTile.pops.Add(this);
            assignToState(newTile.state);
        }
    }

    public void mergePopsInTile(){
        foreach (Pop pop in home.pops){
            if (pop.culture == culture && pop.state == state && pop.troop == troop && pop.home == home){
                pop.changePopulation(population);
                changePopulation(-population);
                break;
            }
        }
    }

    public void SplitPop(int amount, Troop newTroop, Tile newHome){
        if (population - amount > 1){
            changePopulation(-amount);

            Pop newPop =  new Pop();

            newPop.assignToTile(newHome);
            newPop.assignToTroop(newTroop);

            newPop.changePopulation(amount);
        }
    }

    public void changePopulation(int amount){
        population += amount;
        if (population < 1){

            int clampedAmount = amount - Mathf.Abs(population);
            home.totalPopulation += clampedAmount;

            if (state != null){
                state.population += clampedAmount;
            }
            if (troop != null){
                troop.soldiers += clampedAmount;
            }
            population = 0;
            home.pops.Remove(this);
            } else {
                home.totalPopulation += amount;
            if (state != null){
                state.population += amount;
            }
            if (troop != null){
                troop.soldiers += amount;
            }
        }
    }
}
