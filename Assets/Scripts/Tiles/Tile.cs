using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Transform GetTransform;
    public Nation nation;
    public Vector2Int tilePos;
    public GenerateWorld world;
    public Dictionary<Vector2Int, Tile> tileDict;
    public Dictionary<Vector2Int, Tile> borderingTiles;

    public bool tileInitialized = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void TileInit(){
        world = FindAnyObjectByType<GenerateWorld>();

        if (world != null){
            tileDict = world.tileDict;
            for (int x = -1; x < 1; x++){
                for (int y = -1; y < 1; y++){
                    if (new Vector2Int(x,y) != Vector2Int.zero){
                        Vector2Int offsetPos = new Vector2Int(x,y);
                        Vector2Int borderPos = offsetPos + tilePos;

                        bool withinX = !(borderPos.x < 0 || borderPos.x > world.worldSize.x);
                        bool withinY = !(borderPos.y < 0 || borderPos.y > world.worldSize.x);

                        if (withinX && withinY){
                            Tile tile = tileDict[borderPos];
                            if (tile && tile != this){
                                print("yay");
                                print(tile);
                                //borderingTiles.Add(offsetPos, tile);
                                //print(borderingTiles[offsetPos]);
                            }
                        }        
                    }    
                }
            }
            tileInitialized = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (tileInitialized && 1 == 0){
            foreach (KeyValuePair<Vector2Int, Tile> entry in borderingTiles){
                Tile tile = entry.Value;
                if (!tile.nation){
                    tile.nation = nation;
                }
            }  
        }
        
    }
}
