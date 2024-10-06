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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (tilePrefab && tilePrefab.name == "Tile"){
             generateWorld();
             //addRandomNations(1);
        }
    }

    void generateWorld(){
        for (int x = 0; x <= worldSize.x; x++){
            for (int y = 0; y <= worldSize.y; y++){
                // Gets grid position of new tile
                Vector2Int tilePos = new Vector2Int(x,y);
                //print(tilePos);
                
                // Instiates a new tile
                Tile newTile = Instantiate(tilePrefab);
                Transform tileTransform = newTile.GetComponent<Transform>();

                tileTransform.position = new Vector2(x - (worldSize.x/2),y - (worldSize.y/2));
                // Gives tiles a random color to check for overlaps
                //newTile.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                tileTransform.SetParent(this.transform);
                
                // Puts tile in grid
                tileDict.Add(tilePos, newTile);
            }
        }

    }
    void addRandomNations(int amount = 1){
        for (int i = 0; i <= amount; i++){
            Nation newNation = Instantiate(nationPrefab);
            Tile selectedTile = tileDict[new Vector2Int(Random.Range(0, worldSize.x), Random.Range(0, worldSize.y))];
            print(selectedTile);
            // while (!selectedTile.nation){
            //     selectedTile = tileDict[new Vector2Int(Random.Range(0, worldSize.x), Random.Range(0, worldSize.y))];
            // }
            newNation.RandomizeNation();
            newNation.GetComponent<Transform>().SetParent(GameObject.FindWithTag("NationHolder").GetComponent<Transform>());
                
            selectedTile.nation = newNation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
