using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Culture
{
    public string cultureName;
    public Color cultureColor;

    public static Culture createRandomCulture(){
        return new Culture(){
            cultureName = "Fearian",
            cultureColor = new Color(Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f))
        };
    }
}
