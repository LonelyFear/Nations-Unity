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
    public int population;

    public string[] nationNames;
    public string[] nationGovernments;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RandomizeNation(){
        nationColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        if (nationNames.Length > 0 && nationGovernments.Length > 0){
            string name = nationNames[Random.Range(0, nationNames.Length - 1)];
            string govt = nationGovernments[Random.Range(0, nationGovernments.Length - 1)];
            nationName = name + " " + govt;
        }
    }
    public void addTile(Tile tile){
        tiles.Add(tile);
        population += tile.tilePop.population;
    }
    public void removeTile(Tile tile){
        tiles.Remove(tile);
        population -= tile.tilePop.population;
    }
}
