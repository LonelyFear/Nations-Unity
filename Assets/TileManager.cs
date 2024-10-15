using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Unity.Collections;

public class TileManager : MonoBehaviour
{
    public Tilemap tilemap;

    public Nation nationPrefab;
    public Dictionary<Vector3Int, Tile> tiles = new Dictionary<Vector3Int, Tile>();

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

            if (tile.owner != null){
                
                for (int xd = -1; xd <= 1; xd++){
                    for (int yd = -1; yd <= 1; yd++){
                        Vector3Int pos = new Vector3Int(xd,yd) + entry.Key;
                        if (tiles.ContainsKey(pos)){
                            bool canExpand = Random.Range(0f, 1f) < expandChance * tiles[pos].terrain.neutralExpansionMult;
                            bool claimable = tiles[pos].terrain.claimable && tiles[pos].owner == null;

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
            Tile nationTile = tiles[pos];

            while (nationTile == null || nationTile.owner != null || !nationTile.terrain.claimable){
                pos = new Vector3Int(Random.Range(0, world.worldSize.x), Random.Range(0, world.worldSize.y));
                nationTile = tiles[pos];
                attempts--;

                if (attempts <= 0){
                    print("Attempts ran out");
                    nationTile = null;
                    break;
                }
                
            }
            if (nationTile != null){
                Nation newNation = Instantiate(nationPrefab);
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
        } else {
            tilemap.SetColor(position, tiles[position].terrain.terrainColor);
        }
        
        tilemap.SetTileFlags(position, TileFlags.LockColor);
    }
}
