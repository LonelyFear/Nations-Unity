using UnityEngine;

[CreateAssetMenu(fileName = "ThresholdPreset", menuName = "ScriptableObjects/Worldgen/ThresholdPreset", order = 1)]
public class HeightThresholdPreset : ScriptableObject
{
    [Range(0f, 1f)]
    public float oceanThreshold = 0.4f;
    [Range(0f, 1f)]
    public float mountainTreshold = 0.4f;

    [Range(0.1f, 1f)]
    public float hillTreshold = 0.4f;
}
