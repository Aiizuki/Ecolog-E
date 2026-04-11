using Assets.Components.Audio;
using Assets.Components.Singletons;
using UnityEngine;

namespace Assets.Components.InGame.Player.Scripts
{
	public class PlayerAudioController : MonoBehaviour
	{
		#region Unity Lifecycle

		void Start()
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

		private void PlayStrafeSound()
		{
			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerMovement, this.transform.position);
		}

		private void PlayJumpSound()
		{
			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerMovement, this.transform.position);
		}

		private void PlayCrouchSound()
		{
			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Crouch, this.transform.position);
		}

		private void PlayComponentCollisionSound()
			=> AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ComponentCollision, this.transform.position);

		private void PlayObstacleCollisionSound()
			=> AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ObstacleCollision, this.transform.position);

		private void PlayTrashCollisionSound()
			=> AudioManager.Instance.PlayOneShot(FMODEvents.Instance.TrashCollision, this.transform.position);
	}
}