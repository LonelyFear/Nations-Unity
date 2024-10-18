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


    public Tile tileSelected;

    void Start(){
        gameObject.SetActive(false);
    }
    void Update(){
        if (gameObject.activeInHierarchy){
            if (nationName && borderText){
                nationName.text = tileSelected.owner.nationName;
                borderText.text = "Bordering: " + DisplayBorderingNations();
            }
        }    
    }
    
    string DisplayBorderingNations(){
        String str = "";
        foreach (Nation nation in tileSelected.owner.borderingNations){
            str = str + nation.nationName + ", ";
        }
        if (str.Length > 0){
            return str;
        }
        return "None";
    }
}
