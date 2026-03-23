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

		public bool IsDefinedInScene = false;

		private List<Lane> _laneFull;
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

				_laneFull = null;

				UnityEvents.Instance.ObstacleDestroyedEvent.Invoke(this);
				UnityEvents.Instance.ChunkDestroyedEvent.Invoke(this);
			}
		}

		#endregion Unity Lifecycle

		#region Public Methods

		public bool IsBehindPlayer()
			=> _chunkSettings.DeadZoneZIndex >= transform.position.z;

		public void Spawn(Vector3 position)
		{
			transform.position = position;
			UnityEvents.Instance.GenerateNewObstaclesEvent.Invoke(this);
		}

		public void GiveObstacle(List<Obstacle> lstObstacle)
		{
			_laneFull ??= new();

			for (int i = 0; i < lstObstacle.Count; i++)
			{
				Lane lane = RandomisationHelper.GetRandomItemFromList(_lanes.FindAll(l => !_laneFull.Contains(l)).ToList());
				lane.SpawnObstacle(lstObstacle[i], i, GetDistanceBetweenObstacles());
				if (lane.IsFull())
					_laneFull.Add(lane);
			}
		}

		/// <summary>
		/// Determine the number of <see cref="Obstacle"/> we need to assign to a chunk
		/// </summary>
		public float GetNbObstacle()
		{
			float distanceBetweenObstacles = GetDistanceBetweenObstacles();

			int maxNbObstaclePerLane = Mathf.FloorToInt(GetLaneLength() / distanceBetweenObstacles);
			return maxNbObstaclePerLane * GetNbLanes();
		}

		#endregion Public Methods

		#region Private Methods

		private int GetNbLanes()
			=> _lanes.Count;

		private float GetLaneLength()
		{
			Lane lane = _lanes.FirstOrDefault();

			if (lane == null || lane.transform == null || lane.transform.localScale == null)
				return 0;
			return lane.transform.localScale.z;
		}

		private float GetDistanceBetweenObstacles()
		{
			foreach (int time in _assoTimeWithDistance.Keys)
			{
				if (StatsController.Score > time)
					continue;

				return _assoTimeWithDistance[time];
			}

			throw new UnityException("Distance between obstalces can't be null or less or equal to 0");
		}

		#endregion Private Methods
	}
}