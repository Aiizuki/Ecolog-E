using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChunkSettings", menuName = "Scriptable Objects/ChunkSettings")]
public class ChunkSettings : ScriptableObject
{
	[Header("General")]
	public float DeadZoneZIndex;

	[Header("Obstacle Generation")]
	[Tooltip("List of key timeframes in seconds (after each key, we decrease the distance between two obstacles)")]
	public List<int> LstTimeCheckpoints;
	[Tooltip("Distances between two obstacles (in meters) depending on the upper keyframe")]
	public List<float> LstDitancesBetweenTwoObstacles;
}