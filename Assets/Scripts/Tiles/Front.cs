using System.Collections.Generic;
using UnityEngine;

public class Front
{
    public Dictionary<Tile, Troop> tiles = new Dictionary<Tile, Troop>();

    public void updateFront(){
        foreach (var entry in tiles){
            Tile tile = entry.Key;
            Troop troop = entry.Value;

            if (troop.soldiers <= 0){
                tiles[tile] = null;
            }
        }
    }
}
