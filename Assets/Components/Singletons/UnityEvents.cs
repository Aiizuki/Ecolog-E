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
		public static UnityEvents Instance { get; private set; }

		[HideInInspector] public UnityEvent NewGame;
		[HideInInspector] public UnityEvent GameOver;
		[HideInInspector] public UnityEvent GamePause;
		[HideInInspector] public UnityEvent GameResume;

		#region UI

		public static Action GameOverTransition;
		public static Action ReturnToHome;
		public static Action<int> ScoreUpdate;

		public static Action<EnumUpgradableStat> UpgradeStat;
		public static Action<EnumUpgradableStat> FillStatPanel;

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

		public static Action<State> StateChanged;

		#endregion InGame Events

		#region Player Animator

		public static Action PlayRunAnimation;
		public static Action<bool> PlayDodgeAnimation;
		public static Action PlayDeathAnimation;
		public static Action PlayJumpAnimation;
		public static Action PlayCrouchAnimation;
		public static Action<float> SpeedIncreaseEvent;
		public static Action NotifyDeathAnimationFinishedEvent;

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
			GameOver ??= new UnityEvent();

			GameResume ??= new UnityEvent();
			NewGame ??= new UnityEvent();

			CriticalHealthStart ??= new UnityEvent();
			CriticalHealthEnd ??= new UnityEvent();
		}
	}
}