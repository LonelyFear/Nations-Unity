using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public struct Culture
{
    public Color color;
    public int population;
    public static Culture createRandomCulture(){
        return new Culture(){
            color = new Color(Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f))
        };
    }

    public static bool CheckSimilarity(Culture a, Culture b, float minThreshold = 0.2f){
        bool similarR = Mathf.Abs(a.color.r - b.color.r) < minThreshold;
        bool similarG = Mathf.Abs(a.color.g - b.color.g) < minThreshold;
        bool similarB = Mathf.Abs(a.color.b - b.color.b) < minThreshold;

        return similarR && similarG && similarB;
    }
}
