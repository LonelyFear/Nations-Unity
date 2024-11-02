using UnityEngine;
using Random=UnityEngine.Random;

public class Pop
{
    public int population;
    public Tile home;
    public Nation nation;
    public float wealth;
    float monthyGrowthRate = 0.0003f;
    float mortalityRate = 0.00005f;

    public void migrateToTile(int amount, Tile destination){
        population -= amount;
        Pop popToJoin = null;
        foreach (Pop pop in destination.pops){
            popToJoin = pop;
            continue;
        }
        if (popToJoin != null){
            popToJoin.population += amount;
        } else {
            destination.pops.Add(new Pop(){
                population = amount,
                home = destination,
                nation = destination.owner
            });
        }
    }

    public void growPopulation(){
        nation = home.owner;
        int totalGrowth = Mathf.RoundToInt(population * (monthyGrowthRate - mortalityRate));

        if (Random.Range(0f,1f) < (population * monthyGrowthRate) - Mathf.FloorToInt(population * monthyGrowthRate)){
            totalGrowth += 1;
        }
        changePopulation(totalGrowth);
    }

    public void changePopulation(int amount){
        population += amount;
        if (population < 1){

            int clampedAmount = amount - Mathf.Abs(population);
            home.totalPopulation += clampedAmount;

            if (nation){
                nation.population += clampedAmount;
            }
            
            population = 0;
            home.pops.Remove(this);
            } else {
                home.totalPopulation += amount;
            if (nation){
                nation.population += amount;
            }

        }
    }
}
