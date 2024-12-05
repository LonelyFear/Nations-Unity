using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Tilemaps;

public class Tile
{
    public TileManager tileManager;
    public Terrain terrain;    public State state = null;
    public Vector3Int tilePos;
    public Front front;
    public bool border;
    public bool frontier;
    public bool nationalBorder;
    public bool coastal = false;
    public bool anarchy = false;
    public int carryingCapacity {get; private set;}
    public List<State> borderingStates = new List<State>();
    public List<Tile> borderingTiles = new List<Tile>();

    // Population
    public int population;
    public int workforce;
    public List<Pop> pops = new List<Pop>();
    public const int maxPops = 50;

    // Stats
    public float development;

    public void Tick(){
        if (population > 600){
            float developmentIncrease = (population + 0.001f) / 1000000f;
            //Debug.Log(developmentIncrease);
            development += developmentIncrease;
            if (development > 1){
                development = 1f;
            }
            //Debug.Log(development);
        }
        if (population >= 50 && state == null){
            tileManager.addAnarchy(tilePos);
        } else if (population < 50 && anarchy){
            tileManager.RemoveAnarchy(tilePos);
        }
        PrunePops();
    }

    void PrunePops(){
        if (pops.Count > maxPops){
            Pop smallestPop = new Pop(){
                population = 100000000
            };
            foreach (Pop pop in pops){
                if (pop.population < smallestPop.population){
                    smallestPop = pop;
                }
            }
            if (smallestPop.population < population/pops.Count){
                smallestPop.DeletePop();
            }
        }
    }
    public int GetMaxPopulation(){
        // 10k times the fertility is the maximum population a tile can support
        return Mathf.RoundToInt(10000 * terrain.fertility);
    }

    public void ChangePopulation(int amount){
        population += amount;

        if (state != null){
            // Updates our state
            state.ChangePopulation(amount);
        }
        tileManager.worldPopulation += amount;
        if (tileManager.mapMode == TileManager.MapModes.POPULATION){
            UpdateColor();
        }
    }

    public void ChangeWorkforce(int amount){
        workforce += amount;
        if (state != null){
            state.workforce += amount;
        }
    }

    public void UpdateColor(){
        tileManager.updateColor(tilePos);
    }
}
