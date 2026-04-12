using Assets.Components.Game;
using Assets.Components.Game.Chunks;
using Assets.Components.InGame.Chunks.Interactables.Collectibles;
using Assets.Components.InGame.Chunks.Interactables.Obstacles;
using Assets.Components.InGame.Ennemy;
using Assets.Components.Singletons;
using System.Collections.Generic;
using UnityEngine;
using Component = Assets.Components.InGame.Chunks.Interactables.Collectibles.Component;

namespace Assets.Components.InGame.Chunks.Interactables
{
	public class Spawner : MonoBehaviour
	{
		[SerializeField] private ObjectPoolManager _obstaclePool;
		[SerializeField] private ObjectPoolManager _collectiblePool;
		[SerializeField] private ObjectPoolManager _ennemyPool;
		[SerializeField] private ObjectPoolManager _componentPool;

		private readonly float? _lastEnnemySpawnZPos = null;

		#region Unity Lifecycle

		private void Start()
		{
			InitEvents();
		}

		private void OnDestroy()
		{
			RevokeEvents();
		}

		#endregion Unity Lifecycle

		#region UnityEvents

		private void InitEvents()
		{
			UnityEvents.GenerateNewInteractibles += OnGenerateNewInteractibles;
		}

		private void RevokeEvents()
		{
			UnityEvents.GenerateNewInteractibles -= OnGenerateNewInteractibles;
		}

		#endregion UnityEvents

		private void OnGenerateNewInteractibles(Chunk chunk)
		{
			List<AInteractable> lstInteractables = new();

			SpawnObstacles(chunk, ref lstInteractables);
			SpawnCollectibles(chunk, ref lstInteractables);
			SpawnComponent(ref lstInteractables);
			EnnemyController ennemy = SpawnEnnemy(chunk);

			if (lstInteractables.Count == 0)
			{
				Debug.LogWarning("This chunk will recieve no interactables, there must be an issue with its config");
				return;
			}

			chunk.GenerateInLanes(lstInteractables);
			chunk.SpawnEnnemies(ennemy);
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

		private void SpawnComponent(ref List<AInteractable> lstInteractables)
		{
			GameObject obj = _componentPool.Get();

			if (obj == null)
				return;

			Component component = obj.GetComponent<Component>();

			component.SetPool(this._componentPool);
			lstInteractables.Add(component);
		}

#nullable enable
		private EnnemyController? SpawnEnnemy(Chunk chunk)
		{
			if (_lastEnnemySpawnZPos != 0 && _lastEnnemySpawnZPos < chunk.transform.position.z + chunk.GetDistanceBetweenEnnemies())
				return null;

			GameObject obj = _ennemyPool.Get();
			if (obj == null)
				return null;

			EnnemyController ennemy = obj.GetComponent<EnnemyController>();
			ennemy.SetPool(this._ennemyPool);
			return ennemy;
		}
	}
#nullable disable
}