using Assets.Components.Singletons;
using Assets.Components.StateMachines;
using Assets.Components.StateMachines.States;
using Assets.Settings.Player;
using System.Collections;
using UnityEngine;

namespace Assets.Components.InGame.Player.Scripts
{
	public class PlayerInvincibleController : MonoBehaviour
	{
		[SerializeField] private Collider _playerCollider;
		[SerializeField] private PlayerConfig _playerConfig;
		[SerializeField] private GameStateController _gameStateController;

		#region Unity Lifecycle

		private void Start()
		{
			InitEvents();
		}

		private void OnDestroy()
		{
			RevokeEvents();
		}

		#endregion Unity Lifecycle

		#region UnityEvents

		private void InitEvents()
		{
			UnityEvents.StateChanged += OnStateChanged;
		}

		private void RevokeEvents()
		{
			UnityEvents.StateChanged -= OnStateChanged;
		}

		#endregion UnityEvents

		private void OnStateChanged(State newState)
		{
			if (newState is InvincibleState)
				StartCoroutine(InvicibleRoutine());
		}

		private IEnumerator InvicibleRoutine()
		{
			Debug.Log("Player is now invincible");
			// TODO : faire une animation d'invincibilité
			yield return new WaitForSeconds(_playerConfig.InvincibilityDuration);
			_gameStateController.RevertState();
			Debug.Log("Player is no more invincible");
		}
	}
}
