using Assets.Components.Game;
using Assets.Components.Singletons;
using Assets.Components.StateMachines;
using Assets.Components.StateMachines.States;
using Assets.Settings.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Components.InGame.Player.Scripts
{
	public class PlayerHealthController : MonoBehaviour
	{
		[SerializeField] private Image _playerHealthUI;
		[SerializeField] private PlayerConfig _playerConfig;
		[SerializeField] private GameStateController _gameStateController;
		[SerializeField] private StatsController _statsController;

		public float BaseHealth;
		public float Health;
		public float HealthLooseRatio;

		private Dictionary<int, int> _assoDistanceWithDamage;
		private bool _gameOver;
		private bool _criticalHealthEventFired;

		private float CriticalHealthThreshold => 0.30f * BaseHealth;

		#region Unity Lifecycle

		private void Start()
		{
			if (_playerConfig.lstDistanceCheckpoints.Count != _playerConfig.lstDamagePerCheckpoints.Count)
				throw new UnityException("Checkpoints and DamagePerCheckpoints must have the same Length. Please fix the PlayerConfig");

			_assoDistanceWithDamage = _playerConfig.lstDistanceCheckpoints
				.Zip(_playerConfig.lstDamagePerCheckpoints, (key, value) => new { key, value })
				.ToDictionary(x => x.key, x => x.value);

			BaseHealth = Health = _statsController.GetPlayerHealth(_playerConfig.MaxHealth);
			HealthLooseRatio = _statsController.GetPlayerHealthLooseRatio(_playerConfig.HealthLooseRatio);

			SetHealthBarScale(1f);
			InitEvents();
		}

		private void Update()
		{
			if (Health > _playerConfig.MinHealth)
			{
				UpdateHealth();
				UpdateCriticalHealthState();
				SetHealthBarScale(Health / BaseHealth);
			}
			else if (!_gameOver)
			{
				TriggerGameOver();
			}
		}

		private void OnDestroy() => RevokeEvents();

		#endregion Unity Lifecycle

		#region UnityEvents

		private void InitEvents()
		{
			UnityEvents.HealthGain += OnHealthGain;
			UnityEvents.HealthLoose += OnHealthLoose;
			UnityEvents.Instance.GameOver.AddListener(OnGameOver);
		}

		private void RevokeEvents()
		{
			UnityEvents.HealthGain -= OnHealthGain;
			UnityEvents.HealthLoose -= OnHealthLoose;
			UnityEvents.Instance.GameOver.RemoveListener(OnGameOver);
		}

		#endregion UnityEvents

		private void UpdateHealth()
		{
			Health = Mathf.Max(_playerConfig.MinHealth, Health - (HealthLooseRatio / _playerConfig.HealthLooseRate) * Time.deltaTime);
		}

		private void UpdateCriticalHealthState()
		{
			if (Health < CriticalHealthThreshold && !_criticalHealthEventFired)
			{
				UnityEvents.Instance.CriticalHealthStart.Invoke();
				_criticalHealthEventFired = true;
			}
			else if (Health >= CriticalHealthThreshold && _criticalHealthEventFired)
			{
				UnityEvents.Instance.CriticalHealthEnd.Invoke();
				_criticalHealthEventFired = false;
			}
		}

		private void SetHealthBarScale(float scaleX)
		{
			_playerHealthUI.transform.localScale = new Vector3(scaleX, _playerHealthUI.transform.localScale.y, _playerHealthUI.transform.localScale.z);
		}

		private void TriggerGameOver()
		{
			_gameStateController.ChangeState(typeof(GameOverState));
			_gameOver = true;
		}

		private int GetDamageFromDistance()
		{
			foreach (int distance in _assoDistanceWithDamage.Keys)
			{
				if (StatsController.Score <= distance)
					return _assoDistanceWithDamage[distance];
			}
			return 0;
		}

		#region Event Handlers

		private void OnHealthGain(int? amount = 0)
		{
			Health += amount > 0 ? amount.Value : _playerConfig.HealthGainPerTrashCollect;
			Health = Mathf.Min(Health, BaseHealth);
		}

		private void OnHealthLoose(int? amount = 0)
		{
			if (_gameStateController.GetCurrentState() is InvincibleState)
				return;

			int damage = (amount == null || amount <= 0) ? GetDamageFromDistance() : amount.Value;
			float reduction = _statsController.GetPlayerDamageReduction();
			Health = Mathf.Max(_playerConfig.MinHealth, Health - damage * (1f - reduction));

			Debug.Log($"Player lost {damage} hp");

			if (Health <= _playerConfig.MinHealth && !_gameOver)
				TriggerGameOver();
			else
				_gameStateController.ChangeState(typeof(InvincibleState));
		}

		private void OnGameOver()
			=> SetHealthBarScale(0f);

		#endregion Event Handlers
	}
}