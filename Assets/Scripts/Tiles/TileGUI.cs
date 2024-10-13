using UnityEngine;

public class TileGUI : MonoBehaviour
{    
    public bool selectable;
    Tile tile;
    TilePop tilePop;
    void OnMouseOver(){
        if (Input.GetMouseButtonDown(0)){
            print(tile.nation.nationName);
        }
    }

    public void Init(){
        selectable = true;
        tile = GetComponent<Tile>();
    }
}
