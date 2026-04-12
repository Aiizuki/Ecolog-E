using Assets.Components.Audio;
using Assets.Components.Singletons;
using Assets.Components.StateMachines;
using Assets.Components.StateMachines.States;
using UnityEngine;

namespace Assets.Components.InGame.Player.Scripts
{
	public class PlayerAudioController : MonoBehaviour
	{
		[SerializeField] private GameStateController _gameStateController;

		#region Unity Lifecycle

		private void Start()
		{
			InitEvents();
			AudioManager.Instance.GameAmbiance();
		}

		private void OnDestroy()
		{
			RevokeEvents();
		}

		#endregion Unity Lifecycle

		#region Unity Events

		private void InitEvents()
		{
			UnityEvents.OnComponentCollected += PlayComponentCollisionSound;
			UnityEvents.OnTrashCollected += PlayTrashCollisionSound;
			UnityEvents.OnObstacleCollision += PlayObstacleCollisionSound;

			UnityEvents.OnPlayerStrafe += PlayStrafeSound;
			UnityEvents.OnPlayerJump += PlayJumpSound;
			UnityEvents.OnPlayerCrouch += PlayCrouchSound;
		}

		private void RevokeEvents()
		{
			UnityEvents.OnComponentCollected -= PlayComponentCollisionSound;
			UnityEvents.OnTrashCollected -= PlayTrashCollisionSound;
			UnityEvents.OnObstacleCollision -= PlayObstacleCollisionSound;

			UnityEvents.OnPlayerStrafe -= PlayStrafeSound;
			UnityEvents.OnPlayerJump -= PlayJumpSound;
			UnityEvents.OnPlayerCrouch -= PlayCrouchSound;
		}

		#endregion Unity Events

		#region Event Handlers

		private void PlayStrafeSound()
			=> AudioManager.PlayOneShot(FMODEvents.Instance.PlayerMovement, this.transform.position);

		private void PlayJumpSound()
			=> AudioManager.PlayOneShot(FMODEvents.Instance.PlayerMovement, this.transform.position);

		private void PlayCrouchSound()
			=> AudioManager.PlayOneShot(FMODEvents.Instance.Crouch, this.transform.position);

		private void PlayComponentCollisionSound()
			=> AudioManager.PlayOneShot(FMODEvents.Instance.ComponentCollision, this.transform.position);

		private void PlayObstacleCollisionSound()
		{
			if (_gameStateController.GetCurrentState() is not InvincibleState)
				AudioManager.PlayOneShot(FMODEvents.Instance.ObstacleCollision, this.transform.position);
		}

		private void PlayTrashCollisionSound()
			=> AudioManager.PlayOneShot(FMODEvents.Instance.TrashCollision, this.transform.position);

		#endregion Event Handlers
	}
}