using Assets.Components.Game.Chunks;
using Assets.Components.StateMachines.States;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Components.Singletons
{
	/// <summary>
	/// Regroups every Unity Event used in the game
	/// </summary>
	public class UnityEvents : MonoBehaviour
	{
		public static UnityEvents Instance { get; private set; }

		[HideInInspector] public UnityEvent NewGameEvent;
		[HideInInspector] public UnityEvent GameOverEvent;
		[HideInInspector] public UnityEvent GamePauseEvent;
		[HideInInspector] public UnityEvent GameResumeEvent;

		#region UI

		[HideInInspector] public UnityEvent GameOverTransitionEvent;
		[HideInInspector] public UnityEvent ReturnToHomeEvent;
		[HideInInspector] public UnityEvent<int> ScoreUpdateEvent;

		#endregion UI

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
		[HideInInspector] public UnityEvent SpeedIncreaseEvent;

		#endregion InGame Events

		#region Player Animator

		[HideInInspector] public UnityEvent PlayRunAnimation;
		[HideInInspector] public UnityEvent<bool> PlayDodgeAnimation;
		[HideInInspector] public UnityEvent PlayDeathAnimation;
		[HideInInspector] public UnityEvent PlayJumpAnimation;

		#endregion Player Animator

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

			GameResumeEvent ??= new UnityEvent();
			NewGameEvent ??= new UnityEvent();

			SpeedIncreaseEvent ??= new UnityEvent();

			GameOverTransitionEvent ??= new UnityEvent();
			ReturnToHomeEvent ??= new UnityEvent();
			ScoreUpdateEvent ??= new UnityEvent<int>();

			GenerateNewChunkEvent ??= new UnityEvent();
			GenerateNewObstaclesEvent ??= new UnityEvent<Chunk>();
			ChunkDestroyedEvent ??= new UnityEvent<Chunk>();
			ObstacleDestroyedEvent ??= new UnityEvent<Chunk>();

			HealthGainEvent ??= new UnityEvent<int?>();
			HealthLooseEvent ??= new UnityEvent<int?>();
			CriticalHealthEvent ??= new UnityEvent();
			EndCriticalHealthEvent ??= new UnityEvent();

			OnStateChangedEvent ??= new UnityEvent<State>();

			PlayRunAnimation ??= new UnityEvent();
			PlayDodgeAnimation ??= new UnityEvent<bool>();
			PlayDeathAnimation ??= new UnityEvent();
			PlayJumpAnimation ??= new UnityEvent();
		}
	}
}