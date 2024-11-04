using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using Random=UnityEngine.Random;

public class Nation
{
    public string nationName { get; private set; }= "New Nation";
    public Color nationColor { get; private set; } = Color.red;
    public List<Tile> tiles { get; private set; } = new List<Tile>();
    public List<Nation> borderingNations { get; private set; } = new List<Nation>();
    public Dictionary<Nation, Relations> relations { get; private set; } = new Dictionary<Nation, Relations>();
    public int population;
    public TileManager tileManager;

    int weekCounter = 7;

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
        //tileManager = FindAnyObjectByType<TileManager>();
    }

    public static Nation CreateRandomNation(){
        Nation newNation = new Nation(){
            nationColor = new Color(Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f)),
            nationName = "New Nation Empire"
        };
        string[] nationNames = System.IO.File.ReadAllLines("Assets/Text Files/NationNames.txt");
        string[] nationGovernments = System.IO.File.ReadAllLines("Assets/Text Files/TribalGovernments.txt");

        if (nationNames.Length > 0 && nationGovernments.Length > 0){
            string name = nationNames[Random.Range(0, nationNames.Length - 1)];
            string govt = nationGovernments[Random.Range(0, nationGovernments.Length - 1)];
            newNation.nationName = name + " " + govt;
        }
        return newNation;
    }

    public void AddTile(Vector3Int pos){
        Tile tile = tileManager.getTile(pos);
        if (tile.owner != null){
            tile.owner.RemoveTile(pos);
        }
        tiles.Add(tile);
        population += tile.totalPopulation;
        tile.owner = this;
        //Debug.Log(tile.owner.nationName);
        
        tileManager.updateColor(pos);
        tileManager.updateBorders(pos);
    }
    public void RemoveTile(Vector3Int pos){
        Tile tile = tileManager.getTile(pos);
        if (tiles.Contains(tile)){
            tiles.Remove(tile);
            population -= tile.totalPopulation;
            tile.owner = null;

            tileManager.updateColor(pos);
            tileManager.updateBorders(pos);
        }
    }

    public void OnTick(){
        weekCounter--;
        if (weekCounter <= 0){
            weekCounter = 7;
            updateRelations();
        }
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
