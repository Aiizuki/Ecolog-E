using System.Collections.Generic;
using UnityEngine;

namespace Assets.Settings.Chunks
{
	[CreateAssetMenu(fileName = "ChunkSettings", menuName = "Scriptable Objects/ChunkSettings")]
	public class ChunkSettings : ScriptableObject
	{
		[Header("General")]
		public float DeadZoneZIndex;

		[Header("Obstacle Generation")]
		[Tooltip("List of key timeframes in seconds (after each key, we decrease the distance between two obstacles)")] public List<int> LstTimeCheckpoints;
		[Tooltip("Distances between two obstacles (in meters) depending on the upper keyframe")] public List<float> LstDitancesBetweenTwoObstacles;
		[Tooltip("Distance (in meters) before starting spawning obstacles")] public int MinDistanceBeforeObstacleSpawn;

		[Header("Collectibles Settings")]
		[Tooltip("Distances between two collectibles (in meters)")] public float DistanceBetweenCollectibles;
		[Tooltip("Max number of collectibles in each chunk (180m)")] public int MaxCollectiblePerChunk;
		[Tooltip("Distance (in meters) before starting spawning collectibles")] public int MinDistanceBeforeCollectibleSpawn;

		[Header("Ennemy Settings")]
		[Tooltip("List of key distances in meters (after each key, we decrease the distance between two ennemies)")] public List<int> LstDistanceCheckpoints;
		[Tooltip("Distances between two ennemies (in meters) depending on the upper keyframe")] public List<float> LstDitancesBetweenTwoEnnemies;
		[Tooltip("Distance (in meters) before starting spawning ennemies")] public int MinDistanceBeforeEnnemySpawn;
	}
}