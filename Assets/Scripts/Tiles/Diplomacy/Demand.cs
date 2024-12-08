using UnityEngine;
using System.Collections.Generic;

public class Demand : MonoBehaviour
{
    public State demander;
    public State target;
    public List<Tile> annexedTiles = new List<Tile>(); // Tiles directly taken
    public State.StateTypes vassalage = State.StateTypes.INDEPENDENT; // The new status of the target to the demander
    public Dictionary<State, State.StateTypes> demandedVassals = new Dictionary<State, State.StateTypes>(); // Vassals who have their status changed
    bool ally;

    // public List<Tile> puppetedTiles = new List<Tile>();
    // public State.StateTypes integration = State.StateTypes.PUPPET;
}
