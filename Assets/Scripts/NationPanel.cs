using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NationPanel : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nationName;

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
            if (nationName && borderText && popText && sizeText){
                nationName.text = tileSelected.owner.nationName;
                borderText.text = "Relations:" + "<br>" + DisplayBorderingNations();
                popText.text = "Population: " + tileSelected.owner.population.ToString("#,##0");
                sizeText.text = "Tiles: " + tileSelected.owner.tiles.Count.ToString("#,##0 Tiles");
            }
        }    
    }

    string DisplayBorderingNations(){
        String str = "";
        foreach (Nation nation in tileSelected.owner.borderingNations){
            str = str + nation.nationName + ": " + tileSelected.owner.relations[nation].opinion + "<br>";
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
