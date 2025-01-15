using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class Tile
{
    public TileManager tileManager;
    public State state = null;
    public Front front;
    public bool border;
    public bool frontier;
    public bool nationalBorder;
    public List<State> borderingStates = new List<State>();
    public List<Tile> borderingTiles = new List<Tile>();
    // Population
    public List<Pop> pops = new List<Pop>();
    public const int maxPops = 50;

    // Stats & Data
    public Pop rulingPop;
    public TileStruct tileData = new TileStruct();

    public void Tick(){
        GetMaxPopulation();
        if (tileData.population > 600){
            float developmentIncrease = (tileData.population + 0.001f) / 1000000f;
            //Debug.Log(developmentIncrease);
            tileData.development += developmentIncrease;
            if (tileData.development > 1){
                tileData.development = 1f;
            }
            //Debug.Log(development);
        };
        if (tileData.population >= 50 && state == null && !tileData.anarchy){ 
            tileManager.addAnarchy(tileData.tilePos);
        } else if (tileData.population < 50 && tileData.anarchy){
            tileManager.RemoveAnarchy(tileData.tilePos);
        }

        if (tileData.changeAmount != 0){
            ChangePopulation(tileData.changeAmount);
        }
    }
    public void GetMaxPopulation(){
        // 10k times the fertility is the maximum population a tile can support
        tileData.maxPopulation = Mathf.RoundToInt(10000 * tileData.terrain.fertility);
    }


    public void ChangePopulation(int amount){
        tileData.population += amount;

        if (state != null){
            // Updates our state
            state.ChangePopulation(amount);
        }

        if (tileManager.mapMode == TileManager.MapModes.POPULATION){
            UpdateColor();
        }
    }

    public void ChangeWorkforce(int amount){
        tileData.workforce += amount;
        if (state != null){
            state.workforce += amount;
        }
    }

    public void UpdateColor(){
        tileManager.updateColor(tileData.tilePos);
    }
}
public struct TileStruct{
    public int population;
    public int maxPopulation;
    public int workforce;
    public Terrain terrain;  
    public Culture rulingCulture;
    public Culture majorityCulture;
    public Tech tech;
    public float development;
    public UnsafeList<TileStruct> borderingData;
    public UnsafeList<Pop> pops;
    public Vector3Int tilePos;
    public bool coastal;
    public bool anarchy;
    public int changeAmount;
    public void ChangePopulation(int amount){
        changeAmount = amount;
    }  
}