using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Transform GetTransform;
    public Nation nation;

    public Nation newNation;
    public Vector2Int tilePos;
    public GenerateWorld world;
    public Dictionary<Vector2Int, Tile> tileDict;
    public Dictionary<Vector2Int, Tile> borderingTiles = new Dictionary<Vector2Int, Tile>();

    public bool tileInitialized = false;

    public bool interiorTile = false;

    public void TileInit()
    {
        // Subscribes tile to day update
        TimeEvents.dayUpdate += onDayUpdate;
        // Finds the world
        world = FindAnyObjectByType<GenerateWorld>();
        if (world != null){
            // Gets the entire tile dictionary
            tileDict = world.tileDict;

            // Gets the tiles that are adjacent to this one
            getBorderingTiles();

            // Lets the tile know that everything is set up
            tileInitialized = true;
        }
    }
    void getBorderingTiles(){
        for (int x = -1; x <= 1; x++){
            for (int y = -1; y <= 1; y++){
                // Increments for each adjacent tile and checks if the tile isnt Vector2(0,0) which would be self
                 if (new Vector2Int(x,y) != Vector2Int.zero){
                    // Saves the offset pos
                    Vector2Int offsetPos = new Vector2Int(x,y);
                    // Saves the pos of the tile in the tileDict dictionary
                    Vector2Int borderPos = offsetPos + tilePos;

                    bool withinX = borderPos.x >= 0 && borderPos.x < world.worldSize.x;
                    bool withinY = borderPos.y >= 0 && borderPos.y < world.worldSize.y;

                    // Makes sure the tile isnt outside of the dictionary bounds
                    if (withinX && withinY){
                        // Gets the tile at the border position
                        Tile tile = tileDict[borderPos];

                        // One final check to make sure the tile isn't self
                        if (tile != this){
                            /* 
                             Adds that tile to the dictionary with the offset as key.
                             Offset is key so it is easier to check for the tile below you since most tiles are just
                             interacting with adjacent tiles
                             */
                            borderingTiles.Add(offsetPos, tile);
                        }
                    }        
                }    
            }
        }
    }
    public void onDayUpdate(){
        
    }
}
