using Assets.Components.Game;
using Assets.Components.Singletons;
using Assets.Components.StateMachines;
using Assets.Components.StateMachines.States;
using Assets.Settings.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Components.InGame.Player.Scripts
{
	public class PlayerInvincibleController : MonoBehaviour
	{
		[SerializeField] private PlayerConfig _playerConfig;
		[SerializeField] private GameStateController _gameStateController;
		[SerializeField] private StatsController _statsController;

		[SerializeField] private float _blinkInterval = 0.1f;
		[SerializeField] private List<SkinnedMeshRenderer> _lstSkinnedMeshRenderer;

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
			float duration = _statsController.GetPlayerInvicibilityDuration(_playerConfig.InvincibilityDuration);

			Coroutine blinkCoroutine = StartCoroutine(BlinkRoutine());
			yield return new WaitForSeconds(duration);
			StopCoroutine(blinkCoroutine);
			foreach (SkinnedMeshRenderer skinnedMesh in _lstSkinnedMeshRenderer)
				skinnedMesh.enabled = true;

			_gameStateController.RevertState();
			Debug.Log("Player is no more invincible");
		}

		private IEnumerator BlinkRoutine()
		{
			while (true)
			{
				foreach (SkinnedMeshRenderer skinnedMesh in _lstSkinnedMeshRenderer)
					skinnedMesh.enabled = !skinnedMesh.enabled;

				yield return new WaitForSeconds(_blinkInterval);
			}
		}
	}
}
