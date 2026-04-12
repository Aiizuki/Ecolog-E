using Assets.Components.Game.Chunks;
using Assets.Components.PlayerStats;
using Assets.Components.StateMachines.States;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Components.Singletons
{
	/// <summary>
	/// Regroups every Unity Event used in the game
	/// </summary>
	public class UnityEvents : MonoBehaviour
	{
		[Header("Debug")]
		[Tooltip("If true, every event Invoke will be logged")][SerializeField] private bool debugEvents = false;
		[Tooltip("If true, every UI event Invoke will be logged")][SerializeField] private bool debugUIEvents = false;
		[Tooltip("If true, every Animator event Invoke will be logged")][SerializeField] private bool debugAnimatorEvents = false;

		public static UnityEvents Instance { get; private set; }

		[HideInInspector] public UnityEvent NewGame;
		[HideInInspector] public UnityEvent GameOver;

		#region UI

		public static Action GameOverTransition;
		public static Action ReturnToHome;
		public static Action<int> ScoreUpdate;

		public static Action<EnumUpgradableStat> UpgradeStat;
		public static Action<EnumUpgradableStat> FillStatPanel;
		public static Action<EnumUpgradableStat, int> UpdateStatText;

		#endregion UI

		#region InGame Events

		public static Action GenerateNewChunk;
		public static Action<Chunk> GenerateNewInteractibles;
		public static Action<Chunk> ChunkDestroyed;
		public static Action<Chunk> InteractibleDestroyed;

		public static Action<int?> HealthGain;
		public static Action<int?> HealthLoose;
		[HideInInspector] public UnityEvent CriticalHealthStart;
		[HideInInspector] public UnityEvent CriticalHealthEnd;

		public static Action OnTrashCollected;
		public static Action OnComponentCollected;
		public static Action OnObstacleCollision;

		public static Action OnPlayerStrafe;
		public static Action OnPlayerJump;
		public static Action OnPlayerFall;
		public static Action OnPlayerCrouch;

		public static Action<State> StateChanged;

		#endregion InGame Events

		#region Player Animator

		public static Action PlayRunAnimation;
		public static Action<bool> PlayDodgeAnimation;
		public static Action PlayDeathAnimation;
		public static Action PlayCrouchAnimation;
		public static Action<float> SpeedIncreaseEvent;
		public static Action NotifyDeathAnimationFinishedEvent;

		#endregion Player Animator

		#region Unity Lifecycle

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
			if (debugEvents)
				InitListeners();
		}

		#endregion Unity Lifecycle

		private void InitializeEvents()
		{
			GameOver ??= new UnityEvent();
			NewGame ??= new UnityEvent();

			CriticalHealthStart ??= new UnityEvent();
			CriticalHealthEnd ??= new UnityEvent();
		}

		#region Debug

		private void InitListeners()
		{
			NewGame.AddListener(() => DebugEventInvoke(nameof(NewGame)));
			GameOver.AddListener(() => DebugEventInvoke(nameof(GameOver)));

			if (debugUIEvents)
			{
				GameOverTransition += () => DebugEventInvoke(nameof(GameOverTransition));
				ReturnToHome += () => DebugEventInvoke(nameof(ReturnToHome));
				ScoreUpdate += _ => DebugEventInvoke(nameof(ScoreUpdate));
				UpgradeStat += _ => DebugEventInvoke(nameof(UpgradeStat));
				FillStatPanel += _ => DebugEventInvoke(nameof(FillStatPanel));
				UpdateStatText += (EnumUpgradableStat _, int _) => DebugEventInvoke(nameof(UpdateStatText));
			}

			GenerateNewChunk += () => DebugEventInvoke(nameof(GenerateNewChunk));
			GenerateNewInteractibles += _ => DebugEventInvoke(nameof(GenerateNewInteractibles));
			ChunkDestroyed += _ => DebugEventInvoke(nameof(ChunkDestroyed));
			InteractibleDestroyed += _ => DebugEventInvoke(nameof(InteractibleDestroyed));

			HealthGain += _ => DebugEventInvoke(nameof(HealthGain));
			HealthLoose += _ => DebugEventInvoke(nameof(HealthLoose));
			CriticalHealthStart.AddListener(() => DebugEventInvoke(nameof(CriticalHealthStart)));
			CriticalHealthEnd.AddListener(() => DebugEventInvoke(nameof(CriticalHealthEnd)));

			OnTrashCollected += () => DebugEventInvoke(nameof(OnTrashCollected));
			OnComponentCollected += () => DebugEventInvoke(nameof(OnComponentCollected));
			OnObstacleCollision += () => DebugEventInvoke(nameof(OnObstacleCollision));

			OnPlayerStrafe += () => DebugEventInvoke(nameof(OnPlayerStrafe));
			OnPlayerJump += () => DebugEventInvoke(nameof(OnPlayerJump));
			OnPlayerFall += () => DebugEventInvoke(nameof(OnPlayerFall));
			OnPlayerCrouch += () => DebugEventInvoke(nameof(OnPlayerCrouch));

			StateChanged += _ => DebugEventInvoke(nameof(StateChanged));
			SpeedIncreaseEvent += _ => DebugEventInvoke(nameof(SpeedIncreaseEvent));

			if (debugAnimatorEvents)
			{
				PlayRunAnimation += () => DebugEventInvoke(nameof(PlayRunAnimation));
				PlayDodgeAnimation += _ => DebugEventInvoke(nameof(PlayDodgeAnimation));
				PlayDeathAnimation += () => DebugEventInvoke(nameof(PlayDeathAnimation));
				PlayCrouchAnimation += () => DebugEventInvoke(nameof(PlayCrouchAnimation));
				NotifyDeathAnimationFinishedEvent += () => DebugEventInvoke(nameof(NotifyDeathAnimationFinishedEvent));
			}
		}

		private void DebugEventInvoke(string eventName)
			=> Debug.Log($"[UNITY EVENTS] Event {eventName} has been invoked");

		#endregion Debug
	}
}