using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using Random=UnityEngine.Random;

public class Nation : MonoBehaviour
{
    public string nationName = "New Nation";
    public Color nationColor = Color.red;
    public List<Tile> tiles = new List<Tile>();
    public List<Tile> occupiedTiles = new List<Tile>();
    public int population;
    public string[] nationNames;
    public string[] nationGovernments;
    TileManager tileManager;
    void Start(){
        nationInit();
    }

    public void nationInit(){
        tileManager = FindAnyObjectByType<TileManager>();
    }
    public void RandomizeNation(){
        nationColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        if (nationNames.Length > 0 && nationGovernments.Length > 0){
            string name = nationNames[Random.Range(0, nationNames.Length - 1)];
            string govt = nationGovernments[Random.Range(0, nationGovernments.Length - 1)];
            nationName = name + " " + govt;
        }
    }

    public void AddTile(Vector3Int pos){
        Tile tile = tileManager.getTile(pos);
        if (tile.owner){
            tile.owner.RemoveTile(pos);
        }
        tiles.Add(tile);
        population += tile.population;
        tile.owner = this;

        tileManager.updateColor(pos);
        tileManager.updateBorders(pos);
    }

    public void RemoveTile(Vector3Int pos){
        Tile tile = tileManager.getTile(pos);
        if (tiles.Contains(tile)){
            tiles.Remove(tile);
            population -= tile.population;
            tile.owner = null;

            tileManager.updateColor(pos);
            tileManager.updateBorders(pos);
        }
    }
}
