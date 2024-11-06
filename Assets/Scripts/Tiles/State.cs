using System.Collections.Generic;
using UnityEngine;

public class State
{
    public string stateName { get; private set; } = "New State";
    public Tile capital { get; private set; }
    public List<Tile> tiles { get; private set; } = new List<Tile>();
    public int population;
    public TileManager tileManager;
    public Color stateColor = Color.red;

    public Color capitalColor = Color.red;

    // Vassalage
    public State liege;

    public List<State> vassals = new List<State>();
    public StateTypes stateType = StateTypes.INDEPENDENT;
    public enum StateTypes {
        INDEPENDENT,
        PROVINCE,
        COLONY,
        VASSAL
    }

    public List<State> borderingStates { get; private set; } = new List<State>();

    public void getBorders(){
        borderingStates.Clear();
        foreach (Tile tile in tiles){
            if (tile.nationalBorder){
                foreach (State state in tile.borderingStates){
                    if (!borderingStates.Contains(state)){
                        borderingStates.Add(state);
                        // TODO: Add relation stuff here
                    }
                }
            }
        }
    }

    public void AddTile(Vector3Int pos){
        Tile tile = tileManager.getTile(pos);
        tile.anarchy = false;
        if (tile.state != null){
            tile.state.RemoveTile(pos);
        }
        tiles.Add(tile);
        population += tile.totalPopulation;
        tile.state = this;
        //Debug.Log(tile.owner.nationName);
        
        tileManager.updateColor(pos);
        tileManager.updateBorders(pos);
    }

    public void RemoveTile(Vector3Int pos){
        Tile tile = tileManager.getTile(pos);
        if (tiles.Contains(tile)){
            tiles.Remove(tile);
            population -= tile.totalPopulation;
            tile.state = null;

            tileManager.updateColor(pos);
            tileManager.updateBorders(pos);
        }
    }

    public void SetColor(Color color){
        stateColor = color;
        foreach (Tile tile in tiles){
            tileManager.updateColor(tile.tilePos);
        }
    }

    public static State CreateRandomState(){
        State newState = new State(){
            stateColor = new Color(Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f)),
            stateName = "New State"
        };
        string[] stateNames = System.IO.File.ReadAllLines("Assets/Text Files/NationNames.txt");
        string[] stateGovernments = System.IO.File.ReadAllLines("Assets/Text Files/TribalGovernments.txt");

        if (stateNames.Length > 0 && stateGovernments.Length > 0){
            string name = stateNames[Random.Range(0, stateNames.Length - 1)];
            string govt = stateGovernments[Random.Range(0, stateGovernments.Length - 1)];
            newState.stateName = name + " " + govt;
        }
        return newState;
    }

    public void OnTick(){
        updateCapital();
    }

    void updateCapital(){
        switch (stateType){
            case StateTypes.INDEPENDENT:
                capitalColor = Color.red;
                break;
            case StateTypes.PROVINCE:
                capitalColor = new Color(0.5f, 0f, 0.5f, 1f);
                break;
            case StateTypes.COLONY:
                capitalColor = Color.blue;
                break;
            case StateTypes.VASSAL:
                capitalColor = Color.yellow;
                break;
        }
        if (capital == null || capital.state != this){
            capital = null;
            if (tiles.Count > 0){
                capital = tiles[Random.Range(0, tiles.Count - 1)];
                tileManager.updateColor(capital.tilePos);
            }
            
        }
    }

}
