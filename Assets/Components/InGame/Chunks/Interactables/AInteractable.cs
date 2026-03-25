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
			UnityEvents.Instance.InteractibleDestroyedEvent.AddListener(ReturnToPool);
		}

		private void OnDestroy()
		{
			UnityEvents.Instance.InteractibleDestroyedEvent.RemoveListener(ReturnToPool);
		}

		#endregion Unity Lifecycle

		public void SetPool(ObjectPoolManager obstaclePool)
			=> _pool = obstaclePool;

		#region UnityEvents

		private void ReturnToPool(Chunk chunkParent)
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

		#endregion UnityEvents
	}
}
