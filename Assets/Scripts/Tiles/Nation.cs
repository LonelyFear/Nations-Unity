using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;
using Random=UnityEngine.Random;

public class Nation : MonoBehaviour
{
    public string nationName = "New Nation";
    public Color nationColor = Color.red;
    public List<Tile> tiles = new List<Tile>();
    public List<Nation> borderingNations = new List<Nation>();
    public Dictionary<Nation, Relations> relations = new Dictionary<Nation, Relations>();
    public int population;
    public string[] nationNames;
    public string[] nationGovernments;
    TileManager tileManager;
    void Start(){
        nationInit();
    }
    public void getBorders(){
        borderingNations.Clear();
        foreach (Tile tile in tiles){
            if (tile.nationalBorder){
                foreach (Nation nation in tile.borderingNations){
                    if (!borderingNations.Contains(nation)){
                        borderingNations.Add(nation);
                        // If the nation doesnt already have relations, makes new relations
                        if (!relations.ContainsKey(nation)){
                            relations.Add(nation, new Relations());
                        }
                    }
                }
            }
        }
    }

    public void nationInit(){
        gameObject.name = nationName;
        tileManager = FindAnyObjectByType<TileManager>();
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
        tiles.Add(tile);
        population += tile.population;
        tile.owner = this;
        

        tileManager.updateColor(pos);
        tileManager.updateBorders(pos);
        if (tiles.Count > 1){
                gameObject.SetActive(false);
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

    public void onDay(){

    }

    public void updateRelations(){
        foreach (var entry in relations){
            Nation nation = entry.Key;
            Relations relations = entry.Value;
            
            if (borderingNations.Contains(nation)){
                if (Random.Range(0f, 1f) < 0.1){
                    relations.opinion -= 1;
                }

            } else {
                relations.opinion = 0;
            }
            
        }
    }


}
