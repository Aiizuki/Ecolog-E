using Assets.Components.Game;
using Assets.Components.Game.Chunks;
using Assets.Components.Singletons;
using UnityEngine;

namespace Assets.Components.InGame.Chunks.Interactables
{
	public abstract class AInteractable : MonoBehaviour
	{
		public ObjectPoolManager _pool;

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
			UnityEvents.InteractibleDestroyed += OnInteractibleDestroyed;
		}

		private void RevokeEvents()
		{
			UnityEvents.InteractibleDestroyed -= OnInteractibleDestroyed;
		}

		#endregion UnityEvents

		public void SetPool(ObjectPoolManager obstaclePool)
			=> _pool = obstaclePool;

		protected void OnInteractibleDestroyed(Chunk chunkParent)
		{
			if (transform.parent != chunkParent.transform)
				return;

			if (_pool == null)
			{
				Debug.LogError("Obstacle should return to pool but it is null !");
				return;
			}

			_pool.Release(this.gameObject);
		}
	}
}
