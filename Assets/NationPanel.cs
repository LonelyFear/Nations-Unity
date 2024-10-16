using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NationPanel : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nationName;


    public Tile tileSelected;

    void Start(){
        gameObject.SetActive(false);
    }
    void Update(){
        if (gameObject.activeInHierarchy){
            if (nationName){
                nationName.text = tileSelected.owner.nationName;
            }
        }    
    }
}
