using Assets.Components.StateMachines;
using Assets.Components.StateMachines.States;
using Assets.Scripts.Core;
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

		void Start()
		{
			if (_timerSettings == null)
				throw new System.NullReferenceException("Timer settings is not defined !");

			_currentSpeed = _timerSettings.BaseSpeed;
			StartCoroutine(SpeedRoutine());
		}

		private IEnumerator SpeedRoutine()
		{
			while (_currentSpeed < _timerSettings.MaxSpeed && _gameStateController.GetCurrentState() is not GameOverState)
			{
				yield return new WaitForSeconds(_timerSettings.SpeedIncreaseDelay);
				_currentSpeed = Mathf.Min(_currentSpeed + _timerSettings.SpeedIncreaseRate, _timerSettings.MaxSpeed);
				UnityEvents.Instance.SpeedIncreaseEvent.Invoke();
			}
		}

		public static float GetSpeed()
			=> _currentSpeed;
	}
}