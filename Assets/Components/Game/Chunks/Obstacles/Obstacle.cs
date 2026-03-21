using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Components.Game.Chunks.Obstacles
{
	public class Obstacle : MonoBehaviour
	{
		public ObjectPoolManager _pool;

		#region Unity Lifecycle

		private void Start()
		{
			UnityEvents.Instance.ObstacleDestroyedEvent.AddListener(ReturnToPool);
		}

		private void OnDestroy()
		{
			UnityEvents.Instance.ObstacleDestroyedEvent.RemoveListener(ReturnToPool);
		}

		#endregion Unity Lifecycle

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.CompareTag("Player"))
			{
				UnityEvents.Instance.HealthLooseEvent.Invoke(null);
				// TODO : passer la state machine en mode invincible
				_pool.Release(this.gameObject);
			}
		}

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