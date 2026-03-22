using Assets.Components.Game.Chunks;
using Assets.Components.StateMachines.States;
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

		[HideInInspector] public UnityEvent GenerateNewChunkEvent;
		[HideInInspector] public UnityEvent<Chunk> GenerateNewObstaclesEvent;
		[HideInInspector] public UnityEvent<Chunk> ChunkDestroyedEvent;
		[HideInInspector] public UnityEvent<Chunk> ObstacleDestroyedEvent;

		[HideInInspector] public UnityEvent<int?> HealthGainEvent;
		[HideInInspector] public UnityEvent<int?> HealthLooseEvent;
		[HideInInspector] public UnityEvent CriticalHealthEvent;
		[HideInInspector] public UnityEvent EndCriticalHealthEvent;

		[HideInInspector] public UnityEvent<State> OnStateChangedEvent;

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

			GenerateNewChunkEvent ??= new UnityEvent();
			GenerateNewObstaclesEvent ??= new UnityEvent<Chunk>();
			ChunkDestroyedEvent ??= new UnityEvent<Chunk>();
			ObstacleDestroyedEvent ??= new UnityEvent<Chunk>();

			HealthGainEvent ??= new UnityEvent<int?>();
			HealthLooseEvent ??= new UnityEvent<int?>();
			CriticalHealthEvent ??= new UnityEvent();
			EndCriticalHealthEvent ??= new UnityEvent();

			OnStateChangedEvent ??= new UnityEvent<State>();
		}
	}
}