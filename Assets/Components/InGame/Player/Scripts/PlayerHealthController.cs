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

		private float _health;
		private Dictionary<int, int> _assoDistanceWithDamage;

		#region Unity Lifecycle

		private void Start()
		{
			if (_playerConfig.lstDistanceCheckpoints.Count != _playerConfig.lstDamagePerCheckpoints.Count)
				throw new UnityException("Checkpoints and DamagePerCheckpoints must have the same Length. Please fix the PlayerConfig");

			_assoDistanceWithDamage = _playerConfig.lstDistanceCheckpoints
				.Zip(_playerConfig.lstDamagePerCheckpoints, (key, value) => new { key, value })
				.ToDictionary(x => x.key, x => x.value);

			_health = _playerConfig.MaxHealth;
			_playerHealthUI.transform.localScale = new Vector3(1f, _playerHealthUI.transform.localScale.y, _playerHealthUI.transform.localScale.z);

			UnityEvents.Instance.HealthGainEvent.AddListener(Recover);
			UnityEvents.Instance.HealthLooseEvent.AddListener(TakeDamage);
		}

		private void Update()
		{
			if (_health > 0)
			{
				// Réduction progressive de la santé
				_health = Mathf.Max(0, _health - (_playerConfig.HealthLooseRatio / _playerConfig.HealthLooseRate) * Time.deltaTime);

				// TODO : remplacer par un state de la state machine
				if (_health < 0.30f * _playerConfig.MaxHealth)
					UnityEvents.Instance.CriticalHealthEvent.Invoke();
				else
					UnityEvents.Instance.EndCriticalHealthEvent.Invoke();

				// La scale suit _health
				float scaleX = _health / _playerConfig.MaxHealth;
				_playerHealthUI.transform.localScale = new Vector3(scaleX, _playerHealthUI.transform.localScale.y, _playerHealthUI.transform.localScale.z);
			}
			else if (_gameStateController.GetCurrentState() is not GameOverState)
			{
				_playerHealthUI.transform.localScale = new Vector3(0f, _playerHealthUI.transform.localScale.y, _playerHealthUI.transform.localScale.z);
				UnityEvents.Instance.GameOverEvent.Invoke();
			}
		}

		private void OnDestroy()
		{
			UnityEvents.Instance.HealthGainEvent.RemoveListener(Recover);
			UnityEvents.Instance.HealthLooseEvent.RemoveListener(TakeDamage);
		}

		#endregion Unity Lifecycle

		private void Recover(int? amount = 0)
		{
			if (amount > 0)
				_health += amount.Value;
			else
				_health += _playerConfig.HealthGainPerTrashCollect;
		}

		private void TakeDamage(int? amount = 0)
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

			_health = Mathf.Max(_playerConfig.MinHealth, _health - amount.Value);
			Debug.Log($"Player lost {amount.Value} hp");
			_gameStateController.ChangeState(typeof(InvincibleState));
		}
	}
}