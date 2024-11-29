using UnityEngine;

//[CreateAssetMenu(fileName = "TileTerrain", menuName = "ScriptableObjects/Tile/TileTerrain", order = 1)]
public class Terrain
{    
    public float height;
    public float temperature;
    public float moisture;
    public Biome biome;
    public float fertility;
    public float navigability;
    public bool water;
    public bool claimable;

    public enum HeightTypes{
        DEEP_SEA,
        SEA,
        FLAT,
        HILL,
        MOUNTAIN,
    }

    public HeightTypes heightType;

    public void CalcCivStats(){
        water = false;
        claimable = biome.claimable;

        switch (heightType){
            case HeightTypes.SEA:
                fertility = 0;
                navigability = 0;
                water = true;
                claimable = false;
                break;
            case HeightTypes.DEEP_SEA:
                fertility = 0;
                navigability = 0;
                water = true;
                claimable = false;
                break;
            case HeightTypes.FLAT:
                fertility = biome.fertility;
                navigability = biome.navigability;
                break;
            case HeightTypes.HILL:
                fertility = biome.fertility * 0.9f;
                navigability = biome.navigability * 0.8f;
                break;
            case HeightTypes.MOUNTAIN:
                fertility = biome.fertility * 0.8f;
                navigability = biome.navigability * 0.5f;
                break;
        }
    }
}
