using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NationPanel : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI stateName;

    [SerializeField]
    TextMeshProUGUI borderText;

    [SerializeField]
    TextMeshProUGUI popText;

    [SerializeField]
    TextMeshProUGUI sizeText;

    [SerializeField]
    TileManager tm;

    public Tile tileSelected;

    void Start(){
        gameObject.SetActive(false);
    }
    void Update(){
        if (gameObject.activeInHierarchy){

            if (stateName && borderText && popText && sizeText){
                // Gets the state of the selected tile
                State state = tileSelected.state;
                // Gets the population
                int nationalPopulation = state.totalPopulation;
                if (state.liege != null){
                    // If we have a liege sets the national population to that of our liege
                    nationalPopulation = state.liege.totalPopulation;
                }
                stateName.text = state.stateName;
                borderText.text = "Relations:" + "<br>" + displayLiege() + DisplayBorderingNations();
                popText.text = "Local Population: " + state.population.ToString("#,##0") + 
                "<br>National Population: " + nationalPopulation.ToString("#,##0");
                sizeText.text = "Tiles: " + state.tiles.Count.ToString("#,##0 Tiles");
            }
        }    
    }

    string displayLiege(){
        String str = "";
        if (tileSelected.state.liege != null){
            str = "Liege: " + tileSelected.state.liege.stateName + "<br>";
        }
        return str;
    }
    string DisplayBorderingNations(){
        String str = "";

        foreach (State state in tileSelected.state.borderingStates){
            str = str + state.stateName + ": " + tileSelected.state.relations[state].opinion + "<br>";
        }
        if (str.Length > 0){
            return str;
        }
        return "None";
    }

    public void Enable(Tile tile){
        tileSelected = tile;
        gameObject.SetActive(true);
        tm.updateAllColors();
    }

    public void Disable(){
        tileSelected = null;
        gameObject.SetActive(false);
        tm.updateAllColors();
    }
}
