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
		private bool _gameOver = false;
		private bool _criticalHealthEventFired = false;

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

			_playerHealthUI.transform.localScale = new Vector3(1f, _playerHealthUI.transform.localScale.y, _playerHealthUI.transform.localScale.z);

			InitEvents();
		}

		private void Update()
		{
			if (Health > _playerConfig.MinHealth)
			{
				// Réduction progressive de la santé
				Health = Mathf.Max(_playerConfig.MinHealth, Health - (HealthLooseRatio / _playerConfig.HealthLooseRate) * Time.deltaTime);

				if (Health < 0.30f * BaseHealth)
				{
					if (!_criticalHealthEventFired)
					{
						UnityEvents.Instance.CriticalHealthStart.Invoke();
						_criticalHealthEventFired = true;
					}
				}
				else
				{
					UnityEvents.Instance.CriticalHealthEnd.Invoke();
					_criticalHealthEventFired = false;
				}

				// La scale suit _health
				float scaleX = Health / BaseHealth;
				_playerHealthUI.transform.localScale = new Vector3(scaleX, _playerHealthUI.transform.localScale.y, _playerHealthUI.transform.localScale.z);
			}
			else if (!_gameOver)
			{
				UnityEvents.Instance.GameOver.Invoke();
				_gameOver = true;
			}
		}

		private void OnDestroy()
		{
			RevokeEvents();
		}

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

		private void OnHealthGain(int? amount = 0)
		{
			if (amount > 0)
				Health += amount.Value;
			else
				Health += _playerConfig.HealthGainPerTrashCollect;

			Health = Mathf.Min(Health, _playerConfig.MaxHealth);
		}

		private void OnHealthLoose(int? amount = 0)
		{
			if (_gameStateController.GetCurrentState() is InvincibleState)
				return;

			if (amount == null || amount <= 0)
			{
				foreach (int distance in _assoDistanceWithDamage.Keys)
				{
					if (StatsController.Score > distance)
						continue;

					amount = _assoDistanceWithDamage[distance];
					break;
				}
			}

			Health = Mathf.Max(_playerConfig.MinHealth, Health - (amount.Value - (_statsController.GetPlayerDamageReduction() * amount.Value)));
			Debug.Log($"Player lost {amount.Value} hp");

			if (Health <= _playerConfig.MinHealth)
				_gameStateController.ChangeState(typeof(GameOverState));
			else
				_gameStateController.ChangeState(typeof(InvincibleState));
		}

		private void OnGameOver()
			=> _playerHealthUI.transform.localScale = new Vector3(0f, _playerHealthUI.transform.localScale.y, _playerHealthUI.transform.localScale.z);
	}
}