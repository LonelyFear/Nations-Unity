using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Unity.Collections;

public class TileManager : MonoBehaviour
{
    public Tilemap tilemap;

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
        }
        addRandomNations(2);
    }
    public void DayUpdate(){
        neutralExpansion();
    }

    public void neutralExpansion(){
        foreach (var entry in tiles){
            Tile tile = entry.Value;
            float expandChance = 0.1f;

            if (tile.border && tile.owner != null){
                
                for (int xd = -1; xd <= 1; xd++){
                    for (int yd = -1; yd <= 1; yd++){
                        Vector3Int pos = new Vector3Int(xd,yd) + entry.Key;
                        if (tiles.ContainsKey(pos)){
                            bool canExpand = Random.Range(0f, 1f) < expandChance * getTile(pos).terrain.neutralExpansionMult;
                            bool claimable = getTile(pos).terrain.claimable && getTile(pos).owner == null;

                            if (claimable && canExpand){
                                tile.owner.AddTile(pos);
                                // TODO: Make tiles detect if they are a border to improve performance
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

    public void updateColor(Vector3Int position){
        tilemap.SetTileFlags(position, TileFlags.None);

        if (tiles[position].owner){
            tilemap.SetColor(position, tiles[position].owner.nationColor);
            if (tiles[position].border){
                tilemap.SetColor(position, tiles[position].owner.nationColor * 0.7f + Color.black * 0.3f);
            }
        } else {
            tilemap.SetColor(position, tiles[position].terrain.terrainColor);
        }
        
        tilemap.SetTileFlags(position, TileFlags.LockColor);
    }

    // Updates the tile's border bool
    public void Border(Vector3Int position){
        Tile tile = getTile(position);
        if (tile != null){
            tile.border = false;  
            for (int xd = -1; xd <= 1; xd++){
                for (int yd = -1; yd <= 1; yd++){
                    if (yd == 0 && xd == 0){
                        continue;
                    }
                    Vector3Int pos = new Vector3Int(xd,yd) + position;
                    if (getTile(pos) != null && getTile(pos).owner != tile.owner){
                        tile.border = true;
                        updateColor(position);
                        return;
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
}
