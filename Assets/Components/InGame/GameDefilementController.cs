using Assets.Components.Game.Chunks;
using Assets.Components.Singletons;
using Assets.Components.StateMachines.States;
using UnityEngine;

namespace Assets.Components.Game
{
	public class GameDefilementController : MonoBehaviour
	{
		[Header("Parameters")]
		[SerializeField] private GameObject _startPoint;
		[SerializeField] private float _chunkYPos = 0.5f;
		[SerializeField] private GameObject _chunkParent;

		[Header("Components")]
		[SerializeField] private ObjectPoolManager _chunkPool;

		private bool _inGameState;

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
			if (!_inGameState)
				return;

			if (_chunkPool.Pool.Count == 0)
			{
				Debug.LogError("No chunks in pool");
				return;
			}
			GameObject obj = _chunkPool.Get();

			if (obj == null)
				return;

			Chunk chunk = obj.GetComponent<Chunk>();
			chunk.Spawn(new Vector3(position.x, _chunkYPos, position.z), _chunkParent);
		}

		private void OnChunkDestroyed(Chunk chunk)
		{
			if (_chunkPool != null && !chunk.IsDefinedInScene)
				_chunkPool.Release(chunk.gameObject);

			UnityEvents.Instance.GenerateNewChunkEvent.Invoke();
		}

		private void HandleStateChanged(State newState)
		{
			_inGameState = newState is not GameOverState;
		}

		#endregion Private Helpers

		#region UnityEvents

		private void InitEvents()
		{
			UnityEvents.Instance.GenerateNewChunkEvent.AddListener(SpawnChunk);
			UnityEvents.Instance.ChunkDestroyedEvent.AddListener(OnChunkDestroyed);
			UnityEvents.Instance.OnStateChangedEvent.AddListener(HandleStateChanged);
		}

		private void RevokeEvents()
		{
			UnityEvents.Instance.GenerateNewChunkEvent.RemoveListener(SpawnChunk);
			UnityEvents.Instance.ChunkDestroyedEvent.RemoveListener(OnChunkDestroyed);
			UnityEvents.Instance.OnStateChangedEvent.RemoveListener(HandleStateChanged);
		}

		#endregion UnityEvents
	}
}