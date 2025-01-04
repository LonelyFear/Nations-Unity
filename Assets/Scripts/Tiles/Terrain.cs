using Unity.VisualScripting;
using UnityEngine;

//[CreateAssetMenu(fileName = "TileTerrain", menuName = "ScriptableObjects/Tile/TileTerrain", order = 1)]
public class Terrain
{    
    public float height;
    public float temperature;
    public float moisture;
    public Color color;
    public float fertility;
    public float navigability;
    public bool water = false;
    public bool claimable = true;

    public float freezingTemp;
    public float seaLevel;

    public void CalcStats(){
        if (water){
            fertility = 0;
            claimable = false;
            return;
        }
        // Bell curves
        float moistureScore = Mathf.Exp(-Mathf.Pow((moisture - 0.5f) / 0.15f, 2f));
        
        // Temp Score
        float adjustedTemp = Mathf.Clamp01((temperature - freezingTemp)/(1 - freezingTemp));
        float tempScore = Mathf.Exp(-Mathf.Pow((adjustedTemp - 0.5f) / 0.2f, 2f));

        
        // Altitude score
        float adjustedHeight = Mathf.Clamp01((height - seaLevel) / (1f - seaLevel));
        float optimalHeight = 0.3f;
        float altitudeScore = Mathf.Exp(-Mathf.Pow((adjustedHeight - optimalHeight) / 0.15f, 2f));
        // If we are above the optimal height uses a different curve
        if (adjustedHeight > optimalHeight){
            altitudeScore = Mathf.Exp(-Mathf.Pow((adjustedHeight - optimalHeight) / 0.3f, 2f));
        }

        if (temperature > freezingTemp){
            fertility = Mathf.Clamp((moistureScore * tempScore * 0.6f) + (moistureScore * altitudeScore * 0.4f), 0.01f, 1f); 
        } else {
            fertility = moistureScore * 0.01f;
        }
    }
}
