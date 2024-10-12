using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Tile Stats")]
    public Nation nation;
    public TileTerrain terrain;

    [Header("Misc Variables")]
    public Vector2Int tilePos;
    public GenerateWorld world;
    public Dictionary<Vector2Int, Tile> tileDict;
    public Dictionary<Vector2Int, Tile> borderingTiles = new Dictionary<Vector2Int, Tile>();
    public bool border = true;
    public bool costal = false;
    public bool tileInitialized = false;

    [Header("Components")]
    public TilePop tilePop;
    public void TileInit()
    {
        //print("Tile Init Started");
        // Finds the world
        tilePop = GetComponent<TilePop>();
        world = FindAnyObjectByType<GenerateWorld>();
        if (world != null){
            // Gets the entire tile dictionary
            tileDict = world.tileDict;

            // Gets the tiles that are adjacent to this one
            getBorderingTiles();

            // Initializes tile population
            tilePop.initTilePop();

            // Lets the tile know that everything is set up
            tileInitialized = true;
            GetComponent<SendTileData>().init();
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
                    // Makes sure the tile isnt outside of the dictionary bounds
                    if (isWithinWorld(borderPos)){
                        // Gets the tile at the border position
                        Tile tile = tileDict[borderPos];

                        // One final check to make sure the tile isn't self
                        if (tile != this){
                            /* 
                             Adds that tile to the dictionary with the offset as key.
                             Offset is key so it is easier to check for the tile below you since most tiles are just
                             interacting with adjacent tiles
                             */
                            //print(offsetPos);
                            borderingTiles.Add(offsetPos, tile);
                        }
                        // Checks if the tile is ocean
                        if (tile.terrain.naval){
                            // Sets the tile to costal
                            costal = true;
                        }
                    }        
                }    
            }
        }
    }
    public bool isWithinWorld(Vector2Int pos){
        bool withinX = pos.x >= 0 && pos.x < world.worldSize.x;
        bool withinY = pos.y >= 0 && pos.y < world.worldSize.y;
        return withinX && withinY;
    }

    public void onDayUpdate(){
        if (tileInitialized){
            foreach (KeyValuePair<Vector2Int, Tile> valuePair in borderingTiles){
                Tile tile = valuePair.Value;
                float expansionChance = 10 * tile.terrain.neutralExpansionMult;
                if (nation && border && !tile.nation && Random.Range(0,100) < expansionChance && tile.terrain.claimable){
                    tile.changeNation(nation);
                }
            }
        } 
    }

    public bool isBorder(){
        if (nation){
            // Goes through all bordering tiles
            foreach (KeyValuePair<Vector2Int, Tile> valuePair in borderingTiles){
                // Gets the current tile
                Tile tile = valuePair.Value;
                // Checks if the tile is adjacent to a tile of a different nation
                if (tile.nation != nation || tile.nation == null){
                    return true;
                }
            }
        }
        // If the tile is surrounded by other tiles of the same nation, them it returns false
        return false;
    }
    public void changeNation(Nation newNation){
        // Makes sure the tile can be claimed in the first place
        if (terrain.claimable){
            // Checks if the tile already has a nation
            if (nation){
                // If it does, removes the tile from the nation
                nation.removeTile(this);
            }
            // Sets the tile to the new nation
            nation = newNation;
            // Updates the nation with the new tile
            newNation.addTile(this);
            // Updates self
            border = isBorder();
            // Updates nearby tiles
            foreach (KeyValuePair<Vector2Int, Tile> vp in borderingTiles){
                Tile t = vp.Value;
                // Updates nearby tiles to let them know they are borders
                t.border = t.isBorder();
            }
        }
    }
}
