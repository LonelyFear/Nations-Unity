[System.Serializable]
public class Pop
{
    // Population
    public int population;
    public int dependents;
    public int workforce;
    public float workforceRatio = 0.25f;
    const float baseworkforceRatio = 0.25f;
    const float baseBirthRate = 0.04f;
    const float baseDeathRate = 0.036f;

    // Stats
    public Tile tile;
    public State state;
    public Culture culture;
    public Tech tech = new Tech();

    // References
    public PopManager popManager;
    public int index;

    public enum PopStates {
        MIGRATORY,
        SETTLED
    }

    public PopStates status = PopStates.MIGRATORY;
}
