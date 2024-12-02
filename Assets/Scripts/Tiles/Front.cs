using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Front
{
    public State targetState;
    public State state;
    public int manpower;
    public Dictionary<Tile, int> tiles = new Dictionary<Tile, int>();

    public void Tick(){
        RemoveExcessTiles();
    }

    void RemoveExcessTiles(){
        foreach (Tile tile in tiles.Keys.ToArray()){
            if (!tile.borderingStates.Contains(targetState) || tile.state != state || !tile.nationalBorder){
                tile.front = null;
                tiles.Remove(tile);
            } else {
                tile.front = this;
            }
        }
    }
}
