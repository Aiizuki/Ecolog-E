using Assets.Components.Game.Chunks.Lanes;
using Assets.Components.Game.Chunks.Obstacles;
using Assets.Components.Singletons;
using Assets.Scripts.Helpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Components.Game.Chunks
{
	public class Chunk : MonoBehaviour
	{
		[SerializeField] private ChunkSettings _chunkSettings;
		[SerializeField] private List<Lane> _lanes;
		[SerializeField] private float _length;

		public bool IsDefinedInScene = false;
		private Dictionary<int, float> _assoTimeWithDistance;

		#region Unity Lifecycle

		private void Awake()
		{
			if (_chunkSettings.LstTimeCheckpoints.Count != _chunkSettings.LstDitancesBetweenTwoObstacles.Count)
				throw new UnityException("Checkpoints and DistancePerCheckpoint lists must have the same Length. Please fix the ChunkSetting");

			_assoTimeWithDistance = _chunkSettings.LstTimeCheckpoints
				.Zip(_chunkSettings.LstDitancesBetweenTwoObstacles, (key, value) => new { key, value })
				.ToDictionary(x => x.key, x => x.value);
		}

		private void Start()
		{
			if (IsDefinedInScene)
				UnityEvents.Instance.GenerateNewObstaclesEvent.Invoke(this);
		}

		private void Update()
		{
			transform.Translate(ClockManager.GetSpeed() * Time.deltaTime * Vector3.back);
			if (IsBehindPlayer())
			{
				if (IsDefinedInScene)
					Destroy(gameObject);

				UnityEvents.Instance.ObstacleDestroyedEvent.Invoke(this);
				UnityEvents.Instance.ChunkDestroyedEvent.Invoke(this);
			}
		}

		#endregion Unity Lifecycle

		#region Public Methods

		public bool IsBehindPlayer()
			=> _chunkSettings.DeadZoneZIndex >= transform.position.z;

		public void Spawn(Vector3 position, GameObject _chunkParent)
		{
			transform.position = position;
			transform.SetParent(_chunkParent.transform, true);

			UnityEvents.Instance.GenerateNewObstaclesEvent.Invoke(this);
		}

		public void GiveObstacle(List<Obstacle> lstObstacle)
		{
			int maxObstacleLines = lstObstacle.Count / GetNbLanes();
			int obstacleIndex = 0;

			for (int i = 1; i <= maxObstacleLines; i++)
			{
				Lane laneA = RandomisationHelper.GetRandomItemFromList(_lanes);
				Lane laneB = RandomisationHelper.GetRandomItemFromList(_lanes.FindAll(l => l != laneA));

				laneA.SpawnObstacle(lstObstacle[obstacleIndex], i, GetDistanceBetweenObstacles());
				laneB.SpawnObstacle(lstObstacle[obstacleIndex + 1], i, GetDistanceBetweenObstacles());
				obstacleIndex += 2;
			}
		}

		/// <summary>
		/// Determine the number of <see cref="Obstacle"/> we need to assign to a chunk
		/// </summary>
		public int GetNbObstacle()
			=> GetMaxObstaclePerLane() * GetNbLanes();

		#endregion Public Methods

		#region Private Methods

		private int GetNbLanes()
			=> _lanes.Count;

		private float GetDistanceBetweenObstacles()
		{
			if (StatsController.InGameTime > _assoTimeWithDistance.Keys.Last())
				return _assoTimeWithDistance.Keys.Last();

			int lastKey = _assoTimeWithDistance.Keys.First();
			foreach (int time in _assoTimeWithDistance.Keys)
			{
				if (StatsController.InGameTime > time)
				{
					lastKey = time;
					continue;
				}

				return _assoTimeWithDistance[lastKey];
			}

			throw new UnityException("Distance between obstalces can't be null or less or equal to 0");
		}

		private int GetMaxObstaclePerLane()
		{
			float distanceBetweenObstacles = GetDistanceBetweenObstacles();
			return Mathf.FloorToInt(_length / distanceBetweenObstacles);
		}

		#endregion Private Methods
	}
}