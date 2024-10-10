using UnityEngine;

[CreateAssetMenu(fileName = "TileTerrain", menuName = "ScriptableObjects/Tile/TileTerrain", order = 1)]
public class TileTerrain : ScriptableObject
{    
    public bool claimable;
    public bool naval;
    public Color terrainColor;
    [Range(0f, 1f)]
    public float neutralExpansionMult = 1f;
}
