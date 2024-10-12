using UnityEngine;
using UnityEngine.UIElements;

public class SendTileData : MonoBehaviour
{
    Tile tile;
    TilePop tilePop;
    public bool selectable = false;
    
    void OnMouseOver(){
        print("Mouseover");
    }

    public void init(){
        tile = GetComponent<Tile>();
        tilePop = GetComponent<TilePop>();
        selectable = true;
    }
}
