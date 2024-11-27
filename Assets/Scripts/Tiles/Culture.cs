using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Culture
{
    public Color cultureColor;
    public static Culture createRandomCulture(){
        return new Culture(){
            cultureColor = new Color(Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f))
        };
    }
}
