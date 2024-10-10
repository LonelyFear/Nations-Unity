using UnityEngine;

[CreateAssetMenu(fileName = "TileTerrain", menuName = "ScriptableObjects/Tile/TileTerrain", order = 1)]
public class TileTerrain : ScriptableObject
{    
    public bool claimable;
    public bool naval;
    public Color terrainColor;
}
