using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Biome", menuName = "ScriptableObjects/Biome", order = 1)]
public class Biome : ScriptableObject
{    
    [Tooltip("The x axis represents temperature, the y axis reperasents moisture")]
    [Range(0f,1f)]
    public float temperature;
    [Range(0f,1f)]
    public float moisture;
    public bool water;
    public bool claimable;
    [Range(0f,1f)]
    public float fertility;
    [Range(0f,1f)]
    public float navigability;
    public Color biomeColor;
}
