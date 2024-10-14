using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

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
        addRandomNations(20000);
    }
    public void DayUpdate(){

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
