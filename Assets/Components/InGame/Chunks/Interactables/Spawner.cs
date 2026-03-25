using Assets.Components.Game;
using Assets.Components.Game.Chunks;
using Assets.Components.InGame.Chunks.Interactables.Collectibles;
using Assets.Components.InGame.Chunks.Interactables.Obstacles;
using Assets.Components.Singletons;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Components.InGame.Chunks.Interactables
{
	public class Spawner : MonoBehaviour
	{
		[SerializeField] private ObjectPoolManager _obstaclePool;
		[SerializeField] private ObjectPoolManager _collectiblePool;

		#region Unity Lifecycle

		private void Start()
		{
			UnityEvents.Instance.GenerateNewInteractibles.AddListener(SpawnInteractables);
		}

		private void OnDestroy()
		{
			UnityEvents.Instance.GenerateNewInteractibles.RemoveListener(SpawnInteractables);
		}

		#endregion Unity Lifecycle

		private void SpawnInteractables(Chunk chunk)
		{
			List<AInteractable> lstInteractables = new();

			SpawnObstacles(chunk, ref lstInteractables);
			SpawnCollectibles(chunk, ref lstInteractables);
			// TODO : ajouter le super collectible

			if (lstInteractables.Count == 0)
			{
				Debug.LogWarning("This chunk will recieve no interactables, there must be an issue with its config");
				return;
			}

			chunk.GenerateInLanes(lstInteractables);
		}

		private void SpawnObstacles(Chunk chunk, ref List<AInteractable> lstInteractables)
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

			lstInteractables.AddRange(lstObstacle);
		}

		private void SpawnCollectibles(Chunk chunk, ref List<AInteractable> lstInteractables)
		{
			for (int i = 0; i < chunk.GetNbCollectible(); i++)
			{
				GameObject obj = _collectiblePool.Get();

				if (obj == null)
					continue;

				Collectible collectible = obj.GetComponent<Collectible>();

				collectible.SetPool(this._collectiblePool);
				lstInteractables.Add(collectible);
			}
		}
	}
}