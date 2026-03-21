using Assets.Components.Game.Chunks.Lanes;
using Assets.Components.Game.Chunks.Obstacles;
using Assets.Scripts.Core;
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

		#region Unity Lifecycle

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
				lane.SpawnObstacle(lstObstacle[i], i);
				if (lane.IsFull())
					_laneFull.Add(lane);
			}
		}

		/// <summary>
		/// Determine the number of <see cref="Obstacle"/> we need to assign to a chunk
		/// </summary>
		/// <remarks>This is the maximum amount, which assert that the smallest distance between each obstacle on a lane is 1m</remarks>
		public float GetNbObstacle()
			=> GetNbLanes() * GetLaneLength();

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

		#endregion Private Methods
	}
}