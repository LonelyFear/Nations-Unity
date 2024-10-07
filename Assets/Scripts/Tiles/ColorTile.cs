using UnityEngine;

public class ColorTile : MonoBehaviour
{
    Tile tile;

    void Start(){
        tile = GetComponent<Tile>();
    }
    void Update()
    {
        if (tile.nation){
            GetComponent<SpriteRenderer>().color = tile.nation.nationColor;
        } else {
            GetComponent<SpriteRenderer>().color = Color.white;
        }

    }
}
