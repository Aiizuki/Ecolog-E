using Assets.Components.Singletons;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Components.Game.Chunks.Obstacles
{
	public class ObstacleSpawner : MonoBehaviour
	{
		[SerializeField] private ObjectPoolManager _obstaclePool;

		#region Unity Lifecycle

		private void Start()
		{
			UnityEvents.Instance.GenerateNewObstaclesEvent.AddListener(SpawnObstacles);
		}

		private void OnDestroy()
		{
			UnityEvents.Instance.GenerateNewObstaclesEvent.RemoveListener(SpawnObstacles);
		}

		#endregion Unity Lifecycle

		private void SpawnObstacles(Chunk chunk)
		{
			List<Obstacle> lstObstacle = new();
			for (int i = 0; i < chunk.GetNbObstacle(); i++)
			{
				GameObject obj = _obstaclePool.Get(shuffle: true);

				if (obj == null)
					continue;

				Obstacle obstacle = obj.GetComponent<Obstacle>();

				obstacle.SetPool(this._obstaclePool);
				lstObstacle.Add(obstacle);
			}

			if (lstObstacle.Count == 0)
			{
				Debug.LogWarning("This chunk will recieve no obstacle, which means there's an issue with its definition");
				return;
			}

			chunk.GiveObstacle(lstObstacle);
		}
	}
}