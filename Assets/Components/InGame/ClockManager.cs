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
				UnityEvents.Instance.SpeedIncreaseEvent.Invoke();
				//Debug.Log($"[SpeedRoutine] currentSpeed={_currentSpeed} | maxSpeed={_timerSettings.MaxSpeed} | gamePause={_gamePause}");
			}
		}

		private void ResumeRoutine()
		{
			_speedRoutine ??= StartCoroutine(SpeedRoutine());
			_gamePause = false;
		}

		private void PauseRoutine()
		{
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
			UnityEvents.Instance.GameResumeEvent.AddListener(ResumeRoutine);
			UnityEvents.Instance.GameOverEvent.AddListener(PauseRoutine);
			UnityEvents.Instance.GamePauseEvent.AddListener(PauseRoutine);
		}

		private void RevokeEvents()
		{
			UnityEvents.Instance.GameResumeEvent.RemoveListener(ResumeRoutine);
			UnityEvents.Instance.GameOverEvent.RemoveListener(PauseRoutine);
			UnityEvents.Instance.GamePauseEvent.RemoveListener(PauseRoutine);
		}

		#endregion Unity Events
	}
}