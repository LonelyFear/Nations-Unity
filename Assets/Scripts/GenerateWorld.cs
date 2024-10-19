using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateWorld : MonoBehaviour
{
    [Header("Variables And Lists")]
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
    [Tooltip("The noise scale for RANDOM NOISE | Higher scale = Smoother")]
    public int totalNoiseScale;
    [Tooltip("Higher scale = Smoother")]
    public float[] scales = new float[4];
    [Tooltip("Weights the different noise maps")]
    public float[] weights = new float[4];

    void Start()
    {
        if (fixToTexture && preset.noiseTexture){
            fitYToTexture();
        }
        generateWorld();

        // Connects relevant scripts to worldgen finished
        WorldgenEvents.onWorldgenFinished += FindAnyObjectByType<TimeManager>().startTimers;
        TimeEvents.monthUpdate += GetComponent<TileManager>().OnTick;
        GetComponent<TileManager>().Init();

        // Sends worldgen finished signal
        GetComponent<WorldgenEvents>().worldgenFinish();
    }
    void fitYToTexture(){
        float texScale = preset.noiseTexture.Size().x / worldSize.x;
        worldSize.y = Mathf.RoundToInt(preset.noiseTexture.Size().y / texScale);
    }

    float getNoise(int x, int y, float scale, float noiseSeed){
        // Higher scale means less smooth
        var totalScale = scale * totalNoiseScale;
        return Mathf.PerlinNoise((x + 0.1f + noiseSeed)/totalScale,(y + 0.1f + noiseSeed)/totalScale);
    }

    float getHeightNoise(int x, int y){
        float totalNoise;
        // If there isnt a predifined noise texture
        if (!preset.noiseTexture){
            float grains = getNoise(x,y,scales[3], noiseSeed);
            float detail = getNoise(x,y,scales[2], noiseSeed);
            float definition = getNoise(x,y,scales[1], noiseSeed + 300);
            float shape = getNoise(x, y,scales[0], noiseSeed - 2500);

            // Merges the different noise maps and weights them to get more interesting terrain
            totalNoise = (shape * weights[0]) + (definition * weights[1]) + (detail * weights[2]) + (grains * weights[3]);
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
                Vector3Int cellPos = new Vector3Int(x,y);
                float value = getHeightNoise(x,y);

                tilemap.SetTile(cellPos, tileBase);

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
                // Instantiates a tile
                var newTile = new Tile
                {
                    // Sets the tiles terrain
                    terrain = newTileTerrain
                };
                // Adds the tile to the tile manager
                GetComponent<TileManager>().tiles.Add(cellPos, newTile);
                
            }
        }
    }
}
