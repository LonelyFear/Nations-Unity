using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GenerateWorld : MonoBehaviour
{
    [Header("Variables And Lists")]
    public Tile tilePrefab;
    public Nation nationPrefab;
    public Tile[,] tiles;
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
        if (tilePrefab && tilePrefab.name == "Tile"){
            if (fixToTexture && preset.noiseTexture){
                fitYToTexture();
            }
            tiles = new Tile[worldSize.x, worldSize.y];
            generateWorld();
            for (int y = 0; y < worldSize.y; y++){
                for (int x = 0; x < worldSize.x; x++){
                    Tile tile = tiles[x,y];
                    tile.TileInit();
                }
            }
                
             GetComponent<WorldgenEvents>().worldgenFinish();
             //addRandomNations(randomNationCount);
        }
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
                // Sets terrain to default
                TileTerrain newTileTerrain = plains;
                // Checks if the noise value is less than the ocean threshold
                //tilemap.DeleteCells(cellPos);
                tilemap.SetTile(cellPos, tileBase);
                if (value <= preset.oceanThreshold){
                    newTileTerrain = ocean;
                } else if (value > preset.mountainTreshold){
                    newTileTerrain = mountains;
                } else if (value > preset.hillTreshold) {
                    newTileTerrain = hills;
                }
                
                // Gets grid position of new tile
                Vector2Int tilePos = new Vector2Int(x,y);
                //print(tilePos);
                // Instiates a new tile
                Tile newTile = tilemap.GetInstantiatedObject(cellPos).GetComponent<Tile>();
                Transform tileTransform = newTile.transform;
                // (worldSize.x/2) is to align tiles with center of camera
                // 0.5f is to align tile with grid
                //tileTransform.position = new Vector2(x - (worldSize.x/2) + 0.5f, y - (worldSize.y/2) + 0.5f);
                // Gives tiles a random color to check for overlaps
                //newTile.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                //tileTransform.SetParent(transform);
                
                // Puts tile in grid
                tiles[x, y] = newTile;
                // Gives tile terrain
                newTile.terrain = newTileTerrain;
                // Gives Tile its Grid Position
                newTile.tilePos = tilePos;

                // Subscribes tile to onWorldgenFinish Event
                //WorldgenEvents.onWorldgenFinished += newTile.TileInit;
                // Subscribes tile to day update
                //TimeEvents.dayUpdate += newTile.onDayUpdate;
                // Subscribes tilePop to month update
                //TimeEvents.monthUpdate += newTile.GetComponent<TilePop>().onMonthUpdate;
            }
        }
        WorldgenEvents.onWorldgenFinished += FindAnyObjectByType<TimeManager>().startTimers;
    }
    void addRandomNations(int amount = 1){
        if (amount > worldSize.x * worldSize.y){
            amount = worldSize.x * worldSize.y;
        }
        for (int i = 0; i < amount; i++){
            // Instantiates Nation
            Nation newNation = Instantiate(nationPrefab);
            // Picks Random Position
            Vector2Int pos = new Vector2Int(Random.Range(0, worldSize.x), Random.Range(0, worldSize.y));
            // Selects the tile at position
            var selectedTile = tiles[pos.x, pos.y];
            // Checks if the tile is neutral and not ocean
            // NOTE: DO NOT PUT A ! IN FROM OF selectedTile OR GAME WONT LOAD
            int attempts = 0;
            while (selectedTile.nation || !selectedTile.terrain.claimable){
                attempts++;
                if (attempts >= 100){
                    break;
                }
                // Picks a different tile if a nation cant be put there
                 selectedTile = tiles[Random.Range(0, worldSize.x), Random.Range(0, worldSize.y)];
            }
            if (!selectedTile.nation && selectedTile.terrain.claimable){
                // Makes a random nation
                newNation.RandomizeNation();
                // Makes the nation a child of NationHolder
                newNation.GetComponent<Transform>().SetParent(GameObject.FindWithTag("NationHolder").GetComponent<Transform>());
                // Sets the tile to the new nation
                selectedTile.changeNation(newNation);
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
