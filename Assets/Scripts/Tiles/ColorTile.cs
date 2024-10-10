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
            if (tile.border){
                GetComponent<SpriteRenderer>().color = tile.nation.nationColor * 0.8f + Color.black * 0.2f;
            }
        } else {
            GetComponent<SpriteRenderer>().color = tile.terrain.terrainColor;
        }

    }
}
