using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Unity.Collections;
using System;
using Random = UnityEngine.Random;

public class TileManager : MonoBehaviour
{
    public Tilemap tilemap;

    public NationPanel nationPanel;
    public int startingNationCount = 2;
    public Nation nationPrefab;
    public Dictionary<Vector3Int, Tile> tiles = new Dictionary<Vector3Int, Tile>();
    public List<Nation> nations = new List<Nation>();
    GenerateWorld world;

    public void Init(){
        print("Init Ran");
        world = GetComponent<GenerateWorld>();
        foreach (var entry in tiles){
            updateColor(entry.Key);
            entry.Value.tilePos = entry.Key;
            entry.Value.population = Random.Range(50,2000);
        }
        addRandomNations(startingNationCount);
    }
    public void DayUpdate(){
        neutralExpansion();
    }

    public void neutralExpansion(){
        foreach (var entry in tiles){
            Tile tile = entry.Value;
            
            float expandChance = 0.01f;
            if (tile.frontier && tile.owner != null){
                
                for (int xd = -1; xd <= 1; xd++){
                    for (int yd = -1; yd <= 1; yd++){
                        if (Random.Range(0f, 1f) < expandChance){
                            Vector3Int pos = new Vector3Int(xd,yd) + entry.Key;
                            if (tiles.ContainsKey(pos)){
                                    bool canExpand = Random.Range(0f, 1f) < getTile(pos).terrain.neutralExpansionMult;
                                    bool claimable = getTile(pos).terrain.claimable && getTile(pos).owner == null;
                                if (claimable && canExpand){
                                    tile.owner.AddTile(pos);
                                }
                            }
                        }
                        
                        
                    }
                }
            }
        }
    }
    public void addRandomNations(int amount){
        for (int i = 0; i < amount; i++){
            int attempts = 300;
            print("Add nation start");
            Vector3Int pos = new Vector3Int(Random.Range(0, world.worldSize.x), Random.Range(0, world.worldSize.y));
            print(pos);
            Tile nationTile = getTile(pos);

            while (nationTile == null || nationTile.owner != null || !nationTile.terrain.claimable){
                pos = new Vector3Int(Random.Range(0, world.worldSize.x), Random.Range(0, world.worldSize.y));
                nationTile = getTile(pos);
                attempts--;

                if (attempts <= 0){
                    print("Attempts ran out");
                    nationTile = null;
                    break;
                }
                
            }
            if (nationTile != null){
                Nation newNation = Instantiate(nationPrefab);
                if (!nations.Contains(newNation)){
                    nations.Add(newNation);
                }
                newNation.transform.SetParent(GameObject.FindGameObjectWithTag("NationHolder").transform);

                newNation.RandomizeNation();
                newNation.nationInit();

                newNation.AddTile(pos);
            }
                
        }
    }

    public void updateAllColors(){
        foreach (var entry in tiles){
            updateColor(entry.Key);
        }
    }

    public void updateColor(Vector3Int position){
        tilemap.SetTileFlags(position, TileFlags.None);
        Color finalColor;
        if (tiles[position].owner){
            finalColor = tiles[position].owner.nationColor;
            if (tiles[position].border){
                finalColor = tiles[position].owner.nationColor * 0.7f + Color.black * 0.3f;
            }
        } else {
            finalColor = tiles[position].terrain.terrainColor;
        }
        // Higlights selected nation
        if (nationPanel != null && nationPanel.tileSelected != null && nationPanel.tileSelected.owner){
            // Sets the selected nation to the, selected nation
            Nation selectedNation = nationPanel.tileSelected.owner;
            // If the tile isnt the selected nation
            if (tiles[position].owner != selectedNation){
                // Darkens it
                finalColor = finalColor * 0.5f + Color.black * 0.5f;
            }
        }
        tilemap.SetColor(position, finalColor);
        tilemap.SetTileFlags(position, TileFlags.LockColor);
    }

    // Updates the tile's border bool
    public void Border(Vector3Int position){
        Tile tile = getTile(position);
        if (tile != null){
            // If a tile is a border at all
            tile.border = false;
            // If a tile borders a neutral tile
            tile.frontier = false;
            for (int xd = -1; xd <= 1; xd++){
                for (int yd = -1; yd <= 1; yd++){
                    if (yd == 0 && xd == 0){
                        continue;
                    }
                    Vector3Int pos = new Vector3Int(xd,yd) + position;
                    if (getTile(pos) != null && getTile(pos).owner != tile.owner){
                        tile.border = true;
                        if (getTile(pos).owner == null){
                           if (getTile(pos).terrain.claimable){
                                tile.frontier = true;
                            } else if (getTile(pos).terrain.naval){
                                tile.coastal = true;
                            } 
                        }
                        updateColor(position);
                    }
                }
            }
            updateColor(position);
            
        } else {
            return;
        }
    }

    public void updateBorders(Vector3Int position){
        Tile tile = getTile(position);
        if (tile != null){
            for (int xd = -1; xd <= 1; xd++){
                    for (int yd = -1; yd <= 1; yd++){
                        Vector3Int pos = new Vector3Int(xd,yd) + position;
                        if (getTile(pos) != null){
                            Border(pos);
                        }
                    }
                }
        } else {
            return;
        }
    }

    public Tile getTile(Vector3Int position){
        if (tiles.ContainsKey(position)){
            return tiles[position];
        }
        return null;
    }

    void Update(){
        foreach (Nation nation in nations){
            if (nation.tiles.Count < 1){
                nation.gameObject.SetActive(false);
            }
        }
        if (nationPanel){
            detectTileClick();
        }
            
    }

    void detectTileClick(){

        Vector3Int mouseGridPos = tilemap.WorldToCell(FindAnyObjectByType<Camera>().ScreenToWorldPoint(Input.mousePosition));
        if (tiles.ContainsKey(mouseGridPos)){
            Tile tile = tiles[mouseGridPos];
            if (Input.GetMouseButtonDown(0)){
                if (tile != null && tile.owner){
                    nationPanel.tileSelected = tile;
                    nationPanel.gameObject.SetActive(true);
                    updateAllColors();
                } else {
                    nationPanel.tileSelected = null;
                    nationPanel.gameObject.SetActive(false);
                    updateAllColors();
                }
            }
        }
    }
}
