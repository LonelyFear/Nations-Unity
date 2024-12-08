using UnityEngine;

public class Relation
{
    public int opinion = 0;
    public bool atWar = false;
    public float percievedThreat;
    public State.StateTypes relationType;
    public Treaty activeTreaty;
    public Attitudes attitude = Attitudes.CAUTIOUS;
    public enum Attitudes{
        // Negative Attitudes
        DOMINEERING, // Wants to control state
        ANTAGONISTIC, // Wants to align against state
        WARY, // Wants protection from state

        // Neutral Attitudes
        CAUTIOUS, // Random action

        // Positive Attitudes
        PROTECTIVE, // Wants to defend state (Control or defensive pact)
        GENIAL, // Wants to ally state
        COOPERATIVE, // Wants to align with state

        // Subject Attitudes
        LOYAL, // Subjects to demands
        ALOOF, // Wants to keep status quo
        DEFIANT, // Wants to gain autonomy
    }
}
