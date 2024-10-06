using UnityEngine;

public class Tile : MonoBehaviour
{
    public Transform GetTransform;
    public Nation nation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (nation){
            GetComponent<SpriteRenderer>().color = nation.nationColor;
        } else {
            GetComponent<SpriteRenderer>().color = Color.white;
        }

    }
}
