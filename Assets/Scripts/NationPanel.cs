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
                stateName.text = tileSelected.state.stateName;
                borderText.text = "Relations:" + "<br>" + displayLiege() + DisplayBorderingNations();
                popText.text = "Population: " + tileSelected.state.population.ToString("#,##0");
                sizeText.text = "Tiles: " + tileSelected.state.tiles.Count.ToString("#,##0 Tiles");
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
