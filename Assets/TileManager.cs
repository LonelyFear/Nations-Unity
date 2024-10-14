using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TileManager : MonoBehaviour
{
    public Tilemap tilemap;
    public Dictionary<Vector3Int, Tile> tiles = new Dictionary<Vector3Int, Tile>();

    public void dayUpdate(){
        foreach (var entry in tiles){
            updateColor(entry.Key);
        }
    }

    void updateColor(Vector3Int position){
        tilemap.SetTileFlags(position, TileFlags.None);
        tilemap.SetColor(position, tiles[position].terrain.terrainColor);
        tilemap.SetTileFlags(position, TileFlags.LockColor);
    }
}
