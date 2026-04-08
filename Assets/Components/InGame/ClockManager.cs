using Assets.Components.Singletons;
using Assets.Components.StateMachines;
using Assets.Settings.GameDefilement;
using System.Collections;
using UnityEngine;

namespace Assets.Components.Game
{
	public class ClockManager : MonoBehaviour
	{
		[SerializeField] private GameTimerSettings _timerSettings;
		[SerializeField] private GameStateController _gameStateController;

		private static float _currentSpeed = 0f;

		private Coroutine _speedRoutine;
		private float _timer;

		private bool _gamePause = false;
		private int _lastTick;

		#region Unity Lifecycle

		private void Start()
		{
			InitEvents();

			if (_timerSettings == null)
				throw new System.NullReferenceException("Timer settings is not defined !");

			_currentSpeed = _timerSettings.BaseSpeed;
			_speedRoutine = StartCoroutine(SpeedRoutine());
		}

		private void Update()
		{
			if (!_gamePause)
			{
				_timer += Time.deltaTime;
				if (Mathf.FloorToInt(_timer) > _lastTick)
				{
					_lastTick = Mathf.FloorToInt(_timer);
					StatsController.InGameTime = _lastTick;
					StatsController.AddScore(Mathf.FloorToInt(_currentSpeed));
				}
			}
		}

		private void OnDestroy()
		{
			RevokeEvents();
		}

		#endregion Unity Lifecycle

		private IEnumerator SpeedRoutine()
		{
			while (_currentSpeed < _timerSettings.MaxSpeed && !_gamePause)
			{
				yield return new WaitForSeconds(_timerSettings.SpeedIncreaseDelay);
				_currentSpeed = Mathf.Min(_currentSpeed + _timerSettings.SpeedIncreaseRate, _timerSettings.MaxSpeed);
				UnityEvents.SpeedIncreaseEvent.Invoke(_currentSpeed);
				//Debug.Log($"[SpeedRoutine] currentSpeed={_currentSpeed} | maxSpeed={_timerSettings.MaxSpeed} | gamePause={_gamePause}");
			}
		}

		private void OnGameResume()
		{
			_speedRoutine ??= StartCoroutine(SpeedRoutine());
			_gamePause = false;
		}

		private void OnGamePause()
			=> Pause();

		private void OnGameOver()
			=> Pause();

		private void Pause()
		{
			if (_speedRoutine != null)
				StopCoroutine(_speedRoutine);

			_speedRoutine = null;
			_gamePause = true;
		}

		#region Static Helpers

		public static float GetSpeed()
			=> _currentSpeed;

		#endregion Static Helpers

		#region Unity Events

		private void InitEvents()
		{
			UnityEvents.Instance.GameResume.AddListener(OnGameResume);
			UnityEvents.Instance.GameOver.AddListener(OnGameOver);
			UnityEvents.Instance.GamePause.AddListener(OnGamePause);
		}

		private void RevokeEvents()
		{
			UnityEvents.Instance.GameResume.RemoveListener(OnGameResume);
			UnityEvents.Instance.GameOver.RemoveListener(OnGameOver);
			UnityEvents.Instance.GamePause.RemoveListener(OnGamePause);
		}

		#endregion Unity Events
	}
}