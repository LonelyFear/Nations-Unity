using Unity.VisualScripting;
using UnityEngine;

public class TilePop : MonoBehaviour
{
    // Base pop growth per month
    public const float basePopGrowth = 0.0008f;
    public int population;
    public float integration;
    public int troops;

    Tile tile;

    public void initTilePop(){
        tile = GetComponent<Tile>();
        // Initializes tile population
        if (!tile.terrain.naval){
            population = Random.Range(50,3000);
        } 
    }

    public void onMonthUpdate(){
        growPop();
    }

    void growPop(){
        // Gets the initial growth of the population and floors it
        int growthAmount = (int)Mathf.Floor(population * basePopGrowth);
        // Gets the decimal left over from pop growth
        float remainder = (population * basePopGrowth) - growthAmount;
        // Grows the pop by floored amount
        int totalGrowth = growthAmount;
        // Uses the decimal value as a chance for someone to be born
        if (Random.Range(0f, 1f) <= remainder){
            // If the chance is positive grows the population by an additional person
            totalGrowth += 1;
        }
        // Grows tile pop
        changePopulation(totalGrowth);
    }

    void changePopulation(int amount){
        population += amount;
        int nationAmount = amount;

        // Makes sure pop doesnt go below zero
        if (population < 1){
            // If it does, uses the abs of the population and uses it to offset the nation amount
            nationAmount += Mathf.Abs(population);
            // Sets population to 0
            population = 0;
        }

        // Adjusts nation population
        tile.nation.population += nationAmount;
    }
}
