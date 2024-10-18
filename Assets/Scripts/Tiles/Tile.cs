using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class Tile
{
    public TileTerrain terrain;
    public Nation owner;
    public Nation occupier;
    public int population;
    public Vector3Int tilePos;
    public bool border;
    public bool frontier;
    public bool nationalBorder;
    public bool coastal;

    // public void changeOwner(Nation newNation){
    //     // Makes sure the tile can be claimed in the first place
    //     if (terrain.claimable){
    //         // Checks if the tile already has a nation
    //         if (owner){
    //             // If it does, removes the tile from the nation
    //             owner.RemoveTile(tilePos);
    //         }
    //         // Sets the tile to the new nation
    //         owner = newNation;
    //         // Updates the nation with the new tile
    //         newNation.AddTile(tilePos);
    //     }
    // }

}
