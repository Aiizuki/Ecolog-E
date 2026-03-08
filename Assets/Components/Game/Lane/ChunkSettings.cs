using Assets.Components.ObstacleGenerator;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChunkSettings", menuName = "Scriptable Objects/ChunkSettings")]
public class ChunkSettings : ScriptableObject
{
    public float DeadZoneZIndex;

    [Tooltip("Translation speed of chunks in m/s")]
    public float TranslationSpeed = 1f;

    public List<Obstacle> ListObstacle;
}
