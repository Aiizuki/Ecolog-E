using Assets.Components.Game.Chunks;
using Assets.Scripts.Core;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Components.Game
{
	public class GameDefilementController : MonoBehaviour
	{
		[Header("Parameters")]
		[SerializeField] private GameObject _startPoint;
		[SerializeField] private float _chunkYPos = 0.5f;

		[Header("Components")]
		[SerializeField] private ObjectPoolManager _chunkPool;

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

		#region Private Helpers

		private void SpawnChunk()
			=> AddChunk(_startPoint.transform.position);

		private void AddChunk(Vector3 position)
		{
			if (_chunkPool.Pool.Count == 0)
			{
				Debug.LogError("No chunks in pool");
				return;
			}

			GameObject prefab = RandomisationHelper.GetRandomItemFromStack(_chunkPool.Pool);
			GameObject obj = _chunkPool.Get(prefab);

			if (obj == null)
				return;

			Chunk chunk = obj.GetComponent<Chunk>();
			chunk.Spawn(new Vector3(position.x, _chunkYPos, position.z));
		}

		private void OnChunkDestroyed(Chunk chunk)
		{
			if (_chunkPool != null && !chunk.IsDefinedInScene)
				_chunkPool.Release(chunk.gameObject);

			UnityEvents.Instance.GenerateNewChunkEvent.Invoke();
		}

		#endregion Private Helpers

		#region UnityEvents

		private void InitEvents()
		{
			UnityEvents.Instance.GenerateNewChunkEvent.AddListener(SpawnChunk);
			UnityEvents.Instance.ChunkDestroyedEvent.AddListener(OnChunkDestroyed);
		}

		private void RevokeEvents()
		{
			UnityEvents.Instance.GenerateNewChunkEvent.RemoveListener(SpawnChunk);
			UnityEvents.Instance.ChunkDestroyedEvent.RemoveListener(OnChunkDestroyed);
		}

		#endregion UnityEvents
	}
}