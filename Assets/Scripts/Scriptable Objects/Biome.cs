using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "ScriptableObjects/Biome", order = 1)]
public class Biome : ScriptableObject
{    
    [Tooltip("The x axis represents temperature, the y axis reperasents moisture")]
    public float temperature;
    
    public float moisture;
    public bool water;
    public bool claimable;
    public float fertility;
    public Color biomeColor;
}
