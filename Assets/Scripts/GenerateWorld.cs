using System.Net;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateWorld : MonoBehaviour
{
    [Header("Variables And Lists")]
    public Tilemap tilemap;
    public TileBase tileBase;
    
    [Header("World Generation Settings")]
    public Vector2Int worldSize = new Vector2Int(100, 100);

    [Header("Noise Texture Settings")]
    [Tooltip("Overrides all other noise settings")]
    public NoiseMapPreset preset;
    [Tooltip("If true, makes the world size y scale with the proportion to texture y")]
    public bool fixToTexture = true;
    [Header("Random Noise Settings")]
    public float noiseSeed = 0;
    public bool randomizeSeed = false;
    [Tooltip("The noise scale for RANDOM NOISE | Higher scale = Smoother")]
    public int totalNoiseScale;
    [Tooltip("Higher scale = Smoother")]
    public float[] scales = new float[4];
    [Tooltip("Weights the different noise maps")]
    public float[] weights = new float[4];

    [Header("Terrain Settings")]
    public Biome[] biomesToGenerate;
    public Biome oceanBiome;
    public Biome openOceanBiome;
    public Biome deepOceanBiome;
    public Biome seaIceBiome;
    public Biome rock;
    [Range(-0.5f, 0.5f)]
    public float moistureOffset;
    [Range(-0.5f, 0.5f)]
    public float tempOffset;

    void Start()
    {
        if (randomizeSeed){
            noiseSeed = Random.Range(0, 99999);
        }
        if (fixToTexture && preset.noiseTexture){
            fitYToTexture();
        }
        generateWorld();

        // Connects relevant scripts to worldgen finished
        WorldgenEvents.onWorldgenFinished += FindAnyObjectByType<TimeManager>().startTimers;
        TimeEvents.tick += GetComponent<TileManager>().Tick;
        GetComponent<TileManager>().worldSize = worldSize;
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
        var val = Mathf.PerlinNoise((x + 0.1f + noiseSeed)/totalScale,(y + 0.1f + noiseSeed)/totalScale);
        return val;
    }

    float getHeightNoise(int x, int y){
        float totalNoise;
        // If there isnt a predifined noise texture
        if (!preset.noiseTexture){
            
            float grains = getNoise(x,y, scales[3], noiseSeed + 7852);
            float detail = getNoise(x,y,scales[2], noiseSeed);
            float definition = getNoise(x,y,scales[1], noiseSeed + 9830);
            float shape = getNoise(x, y,scales[0], noiseSeed - 3573);

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

    float getMoistureNoise(int x, int y){
        float clouds = getNoise(x,y, 0.05f, noiseSeed + 642);
        float systems = getNoise(x,y, 0.25f, noiseSeed + 753);
        float seas = getNoise(x,y, 0.75f, noiseSeed + 257);
        float shape = getNoise(x, y, 1f, noiseSeed);

        float totalNoise = (shape * 0.6f) + (seas * 0.2f) + (systems * 0.15f) + (clouds * 0.05f) + moistureOffset;
        return totalNoise;
    }

    float getTempRandomNoise(int x, int y){
        float steam = getNoise(x, y, 0.1f, noiseSeed - 332);
        float drifts = getNoise(x, y, 0.5f, noiseSeed - 986);
        float winds = getNoise(x, y, 1f, noiseSeed - 9862);

        return (winds * 0.6f) + (drifts * 0.3f) + (steam * 0.1f);
    }
    float getTemp(int x, int y){
        float equatorPos = worldSize.y / 2;
        float tempValue = 1f - Mathf.Abs(equatorPos - y) / equatorPos;
        tempValue = Mathf.Clamp(tempValue + tempOffset, 0f, 1f);
        return (tempValue * 0.9f) + (getTempRandomNoise(x,y) * 0.1f);
    }

    void generateWorld(){
        float landTiles = 0;
        // Worldsize works like lists, so 0 is the first index and the last index is worldsize - 1
        for (int y = 0; y < worldSize.y; y++){
            for (int x = 0; x < worldSize.x; x++){
                Vector3Int cellPos = new Vector3Int(x,y);

                tilemap.SetTile(cellPos, tileBase);

                // Sets terrain to default
                Terrain terrain = new Terrain(){
                    height = getHeightNoise(x,y),
                    temperature = getTemp(x,y),
                    moisture = getMoistureNoise(x,y)
                };
                

                float minDist = 0.4f;
                Vector2 climatePos = new Vector2(terrain.temperature, terrain.moisture);

                terrain.heightType = Terrain.HeightTypes.FLAT;
                // If we are below the ocean threshold
                if (terrain.height <= preset.oceanThreshold){
                    // Shallow Ocean
                    terrain.biome = oceanBiome;
                    terrain.heightType = Terrain.HeightTypes.SEA;
                    // If we can generate deeper oceans
                    if (preset.oceanThreshold > 0 ){
                        if (terrain.height <= preset.oceanThreshold * 0.65){
                            // Deep Ocean
                            terrain.biome = deepOceanBiome;
                            terrain.heightType = Terrain.HeightTypes.DEEP_SEA;

                        } else if (terrain.height <= preset.oceanThreshold * 0.85){
                            // Open Ocean (Between deep and shallow)
                            terrain.biome = openOceanBiome;
                            terrain.heightType = Terrain.HeightTypes.OPEN_SEA;
                        }
                    }
                    float freezingTemp = (biomesToGenerate[13].temperature + biomesToGenerate[10].temperature)/3;
                    if (getTemp(x,y) <= freezingTemp){
                        terrain.biome = seaIceBiome;
                        terrain.heightType = Terrain.HeightTypes.SEA_ICE;
                    }

                    
                } else {
                    landTiles += 1;
                    Biome chosenBiome = rock;
                    foreach (Biome biome in biomesToGenerate){
                        if (!biome){
                            continue;
                        }
                        var newDist = Vector2.Distance(climatePos, new Vector2(biome.temperature, biome.moisture));
                        if (newDist < minDist){
                            minDist = newDist;
                            chosenBiome = biome;
                        }
                    }

                    terrain.biome = chosenBiome;
                }

                if (terrain.height > preset.mountainTreshold){
                    terrain.heightType = Terrain.HeightTypes.MOUNTAIN;
                } else if (terrain.height > preset.hillTreshold){
                    terrain.heightType = Terrain.HeightTypes.HILL;
                }

                terrain.CalcCivStats();
                
                // Instantiates a tile
                var newTile = new Tile();
                newTile.terrain = terrain;

                for (int ox = -1; ox <= 1; ox++){
                    for (int oy = -1; oy <= 1; oy++){
                        if (getHeightNoise(x + ox, y + oy) < preset.oceanThreshold){
                            newTile.coastal = true;
                            break;
                        }
                    }
                }

                // Adds the tile to the tile manager
                GetComponent<TileManager>().tiles.Add(cellPos, newTile);
                
            }
        }
        print("Land Tiles: " + landTiles);
        print("Total Tiles: " + (worldSize.x * worldSize.y));
        print("Land %: " + Mathf.RoundToInt(landTiles / (worldSize.x * worldSize.y) * 100) + "%");
    }
}
