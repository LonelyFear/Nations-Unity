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

    public Dictionary<Nation, Relations> relations = new Dictionary<Nation, Relations>();
    public int population;
    public string[] nationNames;
    public string[] nationGovernments;
    TileManager tileManager;
    void Start(){
        nationInit();
    }

    public void nationInit(){
        gameObject.name = nationName;
        tileManager = FindAnyObjectByType<TileManager>();
    }

    public void relationUpdate(){
        foreach (Nation nation in tileManager.nations){
            if (nation.gameObject.activeInHierarchy){
                if (relations.ContainsKey(nation)){
                    relations.Remove(nation);
                }
            }
            else if (!relations.ContainsKey(nation)){
                relations.Add(nation, new Relations());
            }

        }
    }
    public void RandomizeNation(){
        nationColor = new Color(Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f));
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
        if (tile.occupier){
            tile.occupier.RemoveOccupation(pos);
        }
        tiles.Add(tile);
        population += tile.population;
        tile.owner = this;
        

        tileManager.updateColor(pos);
        tileManager.updateBorders(pos);
        if (tiles.Count > 1){
                gameObject.SetActive(false);
        }
    }

    public void OccupyTile(Vector3Int pos){
        Tile tile = tileManager.getTile(pos);
        if (tile.occupier){
            tile.occupier.RemoveOccupation(pos);
        }
        occupiedTiles.Add(tile);
        tile.occupier = this;

        tileManager.updateColor(pos);
        tileManager.updateBorders(pos);
    }

    public void RemoveOccupation(Vector3Int pos){
        Tile tile = tileManager.getTile(pos);
        if (occupiedTiles.Contains(tile)){
            tile.occupier = null;

            tileManager.updateColor(pos);
            tileManager.updateBorders(pos);
        }
    }

    public void TakeTile(Vector3Int pos){
        Tile tile = tileManager.getTile(pos);
        if (tile.owner == this && tile.occupier != null){
            tile.occupier.RemoveOccupation(pos);
        }
        else if (tile.owner != null && tile.occupier == null){
            OccupyTile(pos);
        }
    }

    public void RemoveTile(Vector3Int pos){
        Tile tile = tileManager.getTile(pos);
        if (tiles.Contains(tile)){
            tiles.Remove(tile);
            population -= tile.population;
            tile.owner = null;

            tileManager.updateColor(pos);
            tileManager.updateBorders(pos);

            if (tiles.Count < 1){
                gameObject.SetActive(false);
            }
        }
        
    }
}
