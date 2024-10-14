using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateWorld : MonoBehaviour
{
    [Header("Variables And Lists")]
    private Dictionary<Vector3Int, Tile> tiles = new Dictionary<Vector3Int, Tile>();
    public Nation nationPrefab;
    public Tilemap tilemap;
    public TileBase tileBase;
    
    [Header("World Generation Settings")]
    public Vector2Int worldSize = new Vector2Int(100, 100);
    public int randomNationCount = 1;

    [Header("Terrain")]
    public TileTerrain plains;
    public TileTerrain ocean;
    public TileTerrain hills;
    public TileTerrain mountains;

    [Header("Noise Texture Settings")]
    [Tooltip("Overrides all other noise settings")]
    public NoiseMapPreset preset;
    [Tooltip("If true, makes the world size y scale with the proportion to texture y")]
    public bool fixToTexture = true;
    [Header("Random Noise Settings")]
    public float noiseSeed = 0;
    [Tooltip("The noise scale for RANDOM NOISE")]
    public int totalNoiseScale;
    [Tooltip("Higher scale = Less Noisy")]
    public int[] scales = new int[3];
    [Tooltip("Weights the different noise maps")]
    public float[] weights = new float[3];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (fixToTexture && preset.noiseTexture){
            fitYToTexture();
        }
        generateWorld();
        GetComponent<WorldgenEvents>().worldgenFinish();
    }
    void fitYToTexture(){
        float texScale = preset.noiseTexture.Size().x / worldSize.x;
        print(texScale);
        worldSize.y = Mathf.RoundToInt(preset.noiseTexture.Size().y / texScale);
    }
    float getNoise(int x, int y, int scale){
        // Higher scale means less smooth
        return Mathf.PerlinNoise((x + 0.1f + noiseSeed)/scale,(y + 0.1f + noiseSeed)/scale);
    }
    float getHeightNoise(int x, int y){
        float totalNoise;
        // If there isnt a predifined noise texture
        if (!preset.noiseTexture){
            float detail = getNoise(x * totalNoiseScale,y * totalNoiseScale,scales[2]);
            float definition = getNoise(x * totalNoiseScale,y * totalNoiseScale,scales[1]);
            float shape = getNoise(x * totalNoiseScale, y * totalNoiseScale,scales[0]);

            // Merges the different noise maps and weights them to get more interesting terrain
            totalNoise = definition * weights[1] + shape * weights[0] + detail * weights[2];
        } else {
            // Stretches or squashes the values to represent the whole noise texture
            Vector2 texScale = preset.noiseTexture.Size() / worldSize;
            // Gets the pixel pos (Rounds tex scale multiplied by the current x and y)
            Vector2Int pixel = Vector2Int.RoundToInt(new Vector2(x * texScale.x, y * texScale.y));
            // Gets the value of the red channel at the pixel position
            totalNoise = preset.noiseTexture.GetPixel(pixel.x, pixel.y).r;
        }
        return totalNoise;
    }
    void generateWorld(){
        // Worldsize works like lists, so 0 is the first index and the last index is worldsize - 1
        for (int y = 0; y < worldSize.y; y++){
            for (int x = 0; x < worldSize.x; x++){
                Vector3Int cellPos = new Vector3Int(x - (worldSize.x/2),y - (worldSize.y/2));
                float value = getHeightNoise(x,y);

                tilemap.SetTile(cellPos, tileBase);
                print(tilemap.GetTile(cellPos));
                //print(tilemap.GetTile(cellPos).GetTileData(cellPos, tilemap));

                // Sets terrain to default
                TileTerrain newTileTerrain = plains;
                // Checks if the noise value is less than the ocean threshold            
                if (value <= preset.oceanThreshold){
                    newTileTerrain = ocean;
                } else if (value > preset.mountainTreshold){
                    newTileTerrain = mountains;
                } else if (value > preset.hillTreshold) {
                    newTileTerrain = hills;
                }

                var newTile = new Tile();
                newTile.terrain = newTileTerrain;


                GetComponent<TileManager>().tiles.Add(cellPos, newTile);
            }
        }
        WorldgenEvents.onWorldgenFinished += FindAnyObjectByType<TimeManager>().startTimers;
        TimeEvents.dayUpdate += GetComponent<TileManager>().dayUpdate;
    }
}
