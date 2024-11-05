using UnityEngine;
using Random=UnityEngine.Random;

public class Pop
{
    public int population;
    public Tile home;
    public Nation nation;
    public Culture culture;
    public float wealth;
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
                    nation = destination.owner,
                    culture = culture
                };
                newPop.changePopulation(amount);
                destination.pops.Add(newPop);
            }
        }
    }

    public void growPopulation(){
        if (canPopChange){
            float growthRate = baseGrowthRate;
            float mortalityRate = baseMortalityRate;
            if (home.totalPopulation > home.getMaxPopulation()){
                growthRate *= 0.5f - (0.001f * (home.getMaxPopulation() - home.totalPopulation));
            }
           nation = home.owner;
            int totalGrowth = Mathf.RoundToInt(population * (growthRate - mortalityRate));

            if (Random.Range(0f,1f) < (population * growthRate) - Mathf.FloorToInt(population * growthRate)){
                totalGrowth += 1;
            }
            changePopulation(totalGrowth); 
        }
    }

    public void changePopulation(int amount){
        population += amount;
        if (population < 1){

            int clampedAmount = amount - Mathf.Abs(population);
            home.totalPopulation += clampedAmount;

            if (nation != null){
                nation.population += clampedAmount;
            }
            population = 0;
            home.pops.Remove(this);
            } else {
                home.totalPopulation += amount;
            if (nation != null){
                nation.population += amount;
            }
        }
    }
}
