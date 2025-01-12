using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Pop
{
    // Population
    public int population;
    public int dependents;
    public int workforce;
    public float workforceRatio = 0.25f;
    public float birthRate = 0.04f;
    public float deathRate = 0.036f;

    // Stats
    public Tile tile;
    public State state;
    public Culture culture;
    public Tech tech = new Tech();

    // References
    public PopManager popManager;
    public int index;
    public PopManager.PopStates status = PopManager.PopStates.MIGRATORY;

    public void Tick(){
        if (population > 0){
            GrowPopulation();
        }
    }

    void GrowPopulation(){
        float bRate = birthRate/TimeManager.ticksPerYear;
        float dRate = deathRate/TimeManager.ticksPerYear;
        if (population < 2){
            bRate = 0f;
        }
        if (tile.population > 10000){
            bRate *= 0.75f;
        }
        float natutalGrowthRate = bRate - dRate;
        int totalGrowth = Mathf.RoundToInt(population * natutalGrowthRate);
        
        if (Random.Range(0f, 1f) < Mathf.Abs(population * natutalGrowthRate) % 1){
            totalGrowth += (int) Mathf.Sign(natutalGrowthRate);
        }

        popManager.ChangePopulation(this, totalGrowth);
    }
}

public struct PopData{

}
