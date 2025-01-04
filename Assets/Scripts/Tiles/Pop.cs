using UnityEditor.ShaderGraph.Internal;

[System.Serializable]
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

    public static PopStruct ConvertToStruct(Pop pop){
        return new PopStruct{
            population = pop.population,
            dependents = pop.dependents,
            workforce = pop.workforce,
            birthRate = pop.birthRate,
            deathRate = pop.deathRate,
            tech = Tech.ConvertToStruct(pop.tech)
        };
    }
    public static Pop ReturnToClass(PopStruct popStruct, Pop output){
        if (output == null){
            output = new Pop();
        }
        output.population = popStruct.population;
        output.dependents = popStruct.dependents;
        output.workforce = popStruct.workforce;
        output.birthRate = popStruct.birthRate;
        output.deathRate = popStruct.deathRate;
        output.tech = Tech.ReturnToClass(popStruct.tech);
        return output;
    }
}

public struct PopStruct{
    public int population;
    public int dependents;
    public int workforce;
    public float birthRate;
    public float deathRate;
    public TechStruct tech;
}
