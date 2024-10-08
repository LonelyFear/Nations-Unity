using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class GenerateWorld : MonoBehaviour
{
    [Header("Variables And Lists")]
    public Tile tilePrefab;
    public Nation nationPrefab;
    public Dictionary<Vector2Int, Tile> tileDict = new Dictionary<Vector2Int, Tile>();
    
    [Header("World Generation Settings")]
    public Vector2Int worldSize = new Vector2Int(100, 100);
    public int randomNationCount = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (tilePrefab && tilePrefab.name == "Tile"){
             generateWorld();
             addRandomNations(randomNationCount);
             GetComponent<WorldgenEvents>().worldgenFinish();
        }
    }
    void generateWorld(){
        // Worldsize works like lists, so 0 is the first index and the last index is worldsize - 1
        for (int x = 0; x < worldSize.x; x++){
            for (int y = 0; y < worldSize.y; y++){
                // Gets grid position of new tile
                Vector2Int tilePos = new Vector2Int(x,y);
                //print(tilePos);
                
                // Instiates a new tile
                Tile newTile = Instantiate(tilePrefab);
                Transform tileTransform = newTile.GetComponent<Transform>();

                // (worldSize.x/2) is to align tiles with center of camera
                // 0.5f is to align tile with grid
                tileTransform.position = new Vector2(x - (worldSize.x/2) + 0.5f, y - (worldSize.y/2) + 0.5f);

                // Gives tiles a random color to check for overlaps
                //newTile.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                tileTransform.SetParent(this.transform);
                
                // Puts tile in grid
                tileDict.Add(tilePos, newTile);

                // Gives Tile its Grid Position
                newTile.tilePos = tilePos;

                // Subscribes tile to onWorldgenFinishEvent
                WorldgenEvents.onWorldgenFinished += newTile.TileInit;
                // Subscribes tile to day update
                TimeEvents.dayUpdate += newTile.onDayUpdate;
            }
        }

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
            var selectedTile = tileDict[pos];
            // Checks if the tile is neutral and not ocean
            // NOTE: DO NOT PUT A ! IN FROM OF selectedTile OR GAME WONT LOAD
            while (selectedTile.nation){
                // Picks a different tile if a nation cant be put there
                 selectedTile = tileDict[new Vector2Int(Random.Range(0, worldSize.x), Random.Range(0, worldSize.y))];
            }

            // Makes a random nation
            newNation.RandomizeNation();
            // Makes the nation a child of NationHolder
            newNation.GetComponent<Transform>().SetParent(GameObject.FindWithTag("NationHolder").GetComponent<Transform>());
            // Sets the tile to the new nation
            selectedTile.nation = newNation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
