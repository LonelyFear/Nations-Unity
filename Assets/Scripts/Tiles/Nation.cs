using System.Collections.Generic;
using UnityEngine;

public class Nation : MonoBehaviour
{
    public string nationName = "New Nation";
    public Color nationColor = Color.red;
    public Tile[] tiles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         print("Im a nation");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RandomizeNation(){
        nationColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }
}
