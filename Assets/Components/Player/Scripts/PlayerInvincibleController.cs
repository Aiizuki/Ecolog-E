using Assets.Components.StateMachines;
using Assets.Components.StateMachines.States;
using Assets.Scripts.Core;
using Assets.Settings.Player;
using System.Collections;
using UnityEngine;

namespace Assets.Components.Player
{
	public class PlayerInvincibleController : MonoBehaviour
	{
		[SerializeField] private Collider _playerCollider;
		[SerializeField] private PlayerConfig _playerConfig;
		[SerializeField] private GameStateController _gameStateController;

		private void Start()
		{
			UnityEvents.Instance.OnStateChangedEvent.AddListener(HandleStateChange);
		}

		private void HandleStateChange(State newState)
		{
			if (newState is InvincibleState)
				StartCoroutine(InvicibleRoutine());
		}

		private IEnumerator InvicibleRoutine()
		{
			State currentState = _gameStateController.GetCurrentState();

			Debug.Log("Player is now invincible");
			// TODO : faire une animation d'invincibilité
			yield return new WaitForSeconds(_playerConfig.InvincibilityDuration);
			_gameStateController.ChangeState(currentState.GetType());
			Debug.Log("Player is no more invincible");
		}
	}
}
