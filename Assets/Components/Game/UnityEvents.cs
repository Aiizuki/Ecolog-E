using Assets.Components.Game.Chunks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Core
{
	/// <summary>
	/// Regroups every Unity Event used in the game
	/// </summary>
	public class UnityEvents : MonoBehaviour
	{
		public static UnityEvents Instance { get; private set; }

		[HideInInspector] public UnityEvent GameOverEvent;
		[HideInInspector] public UnityEvent GameOverTransitionEvent;
		[HideInInspector] public UnityEvent SpeedIncreaseEvent;
		[HideInInspector] public UnityEvent ReturnToHomeEvent;
		[HideInInspector] public UnityEvent NewGameEvent;

		#region InGame Events

		[HideInInspector] public UnityEvent GenerateNewChunk;
		[HideInInspector] public UnityEvent<Chunk> GenerateNewObstacles;
		[HideInInspector] public UnityEvent<Chunk> ChunkDestroyed;
		[HideInInspector] public UnityEvent<Chunk> ObstacleDestroyed;

		#endregion InGame Events

		void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
			DontDestroyOnLoad(gameObject);

			InitializeEvents();
		}

		private void InitializeEvents()
		{
			GameOverEvent ??= new UnityEvent();
			SpeedIncreaseEvent ??= new UnityEvent();
			ReturnToHomeEvent ??= new UnityEvent();
			NewGameEvent ??= new UnityEvent();
			GameOverTransitionEvent ??= new UnityEvent();
			GenerateNewChunk ??= new UnityEvent();
			GenerateNewObstacles ??= new UnityEvent<Chunk>();
			ChunkDestroyed ??= new UnityEvent<Chunk>();
			ObstacleDestroyed ??= new UnityEvent<Chunk>();
		}
	}
}