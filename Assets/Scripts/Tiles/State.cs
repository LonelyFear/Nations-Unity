using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using Random = UnityEngine.Random;

public class State
{
    public string stateName { get; private set; } = "New State";
    public Tile capital { get; private set; }
    public List<Tile> tiles { get; private set; } = new List<Tile>();
    public int population;
    public TileManager tileManager;
    public Color stateColor = Color.red;
    public Color capitalColor = Color.red;
    public Color mapColor = Color.red;

    // Vassalage & Relations
    public State liege;
    public Dictionary<State, StateTypes> vassals = new Dictionary<State, StateTypes>();
    public Dictionary<State, Relation> relations = new Dictionary<State, Relation>();
    public Dictionary<State, Front> fronts = new Dictionary<State, Front>();
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
                        fixRelations();
                    }
                    if (!fronts.ContainsKey(state)){
                        fronts.Add(state, new Front());
                    }
                    if (!fronts[state].tiles.ContainsKey(tile)){
                        fronts[state].tiles.Add(tile, null);
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
        tileManager.anarchy.Remove(tile);
        
        tileManager.updateColor(pos);
        tileManager.updateBorders(pos);
    }

    public void RemoveTile(Vector3Int pos){
        Tile tile = tileManager.getTile(pos);
        if (tiles.Contains(tile)){
            tiles.Remove(tile);
            population -= tile.totalPopulation;
            tile.state = null;
            tile.anarchy = true;

            tileManager.anarchy.Add(tile);

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

    public void fixFronts(){
        
    }
    
    public void VassalizeState(State state, StateTypes type = StateTypes.VASSAL){
        if (type != StateTypes.INDEPENDENT && !vassals.ContainsKey(state) && liege != state && state.liege != this){
            Debug.Log("Vassalization");

            if (state.liege != null){
                state.liege.ReleaseState(state);   
            }

            if (state.vassals.Count > 0){
                List<State> stateVassals = new List<State>();
                foreach (var entry in state.vassals){
                    State vassalVassal = entry.Key;
                    stateVassals.Add(vassalVassal);
                }
                foreach (State state1 in stateVassals){
                    state.ReleaseState(state1);
                }
                
            }

            state.liege = this;
            vassals.Add(state, type);

            if (!state.relations.ContainsKey(this)){
                state.relations.Add(this, new Relation(){
                    relationType = type
                });
            } else {
                state.relations[this].relationType = type;
            }
            state.updateStateType();
            state.updateCapital();
            state.updateColor();
            state.fixRelations();
            
        }
    }

    public void updateStateType(){
        if (liege != null && relations.ContainsKey(liege)){
            stateType = relations[liege].relationType;
        }
    }

    public void ReleaseState(State state){
        if (vassals.ContainsKey(state)){
            state.relations[this].relationType = StateTypes.INDEPENDENT;
            vassals.Remove(state);
            

            state.updateStateType();
            state.liege = null;

            state.updateCapital();
            state.updateColor();
            state.fixRelations();
        }
    }

    public static State CreateRandomState(){
        State newState = new State(){
            stateColor = new Color(Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f)),
            stateName = "New State"
        };
        string[] stateNames = System.IO.File.ReadAllLines("Assets/Text Files/NationNames.txt");
        string[] stateGovernments = System.IO.File.ReadAllLines("Assets/Text Files/TribalGovernments.txt");

        newState.mapColor = newState.stateColor;

        if (stateNames.Length > 0 && stateGovernments.Length > 0){
            string name = stateNames[Random.Range(0, stateNames.Length - 1)];
            string govt = stateGovernments[Random.Range(0, stateGovernments.Length - 1)];
            newState.stateName = name + " " + govt;
        }
        return newState;
    }

    public void OnTick(){
        if (liege == null){
            //stateType = StateTypes.INDEPENDENT;
        }
        updateCapital();

        foreach (State state in borderingStates){
            if (relations.ContainsKey(state) && state.liege == null && tiles.Count > state.tiles.Count && tiles.Count > 25 && liege == null){
                VassalizeState(state, StateTypes.PROVINCE);
            }
        }
    }

    public void fixRelations(){
        foreach (State border in borderingStates){
            if (!relations.ContainsKey(border)){
                relations.Add(border, new Relation());
            }
        }

        foreach (var entry in relations){
            State state = entry.Key;
            Relation relation = entry.Value;

            // Makes relations with liege
            if (liege != null && !relations.ContainsKey(liege) && !liege.relations.ContainsKey(this)){
                Relation newRelation = new Relation(){
                    relationType = stateType,
                };
            }
            // Removes inactive relations
            if (state.tiles.Count < 1){
                if (liege != null && liege == state){
                    relations.Remove(state);
                }
                relations.Remove(state);
            }
        }
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

    void updateColor(){
        //updateCapital();
        switch (stateType){
            case StateTypes.INDEPENDENT:
                mapColor = stateColor;
                break;
            case StateTypes.PROVINCE:
                mapColor = liege.mapColor;
                break;
            case StateTypes.COLONY:
                mapColor = stateColor * 0.5f + liege.mapColor * 0.5f;;
                break;
            case StateTypes.VASSAL:
                mapColor = stateColor * 0.5f + liege.mapColor * 0.5f;
                break;
        }
        foreach (Tile tile in tiles){
            tileManager.updateColor(tile.tilePos);
        }
    }

}
