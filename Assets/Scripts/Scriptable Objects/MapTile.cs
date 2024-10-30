
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;


#if UNITY_EDITOR
using UnityEditor;
# endif

[CreateAssetMenu(fileName = "MapTile", menuName = "ScriptableObjects/MapTile", order = 2)]
public class MapTile : UnityEngine.Tilemaps.Tile
{
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
    }
}
