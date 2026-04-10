using Assets.Components.Audio;
using Assets.Components.InGame.Chunks.Interactables.Collectibles;
using Assets.Components.InGame.Chunks.Interactables.Obstacles;
using Assets.Components.Singletons;
using System;
using UnityEngine;
using Component = Assets.Components.InGame.Chunks.Interactables.Collectibles.Component;

namespace Assets.Components.InGame.Player.Scripts
{
	public class PlayerAudioController : MonoBehaviour
	{
		#region Unity Lifecycle

		void Start()
		{
			InitEvents();
		}

		private void OnDestroy()
		{
			RevokeEvents();
		}

		#endregion Unity Lifecycle

		#region Unity Events

		private void InitEvents()
		{
			UnityEvents.OnComponentCollected += () => PlayCollisionSound(typeof(Component));
			UnityEvents.OnTrashCollected += () => PlayCollisionSound(typeof(Collectible));
			UnityEvents.OnObstacleCollision += () => PlayCollisionSound(typeof(Obstacle));

			UnityEvents.OnPlayerStrafe += PlayStrafeSound;
			UnityEvents.OnPlayerJump += PlayJumpSound;
			UnityEvents.OnPlayerCrouch += PlayCrouchSound;
		}

		private void RevokeEvents()
		{
			UnityEvents.OnComponentCollected -= () => PlayCollisionSound(typeof(Component));
			UnityEvents.OnTrashCollected -= () => PlayCollisionSound(typeof(Collectible));
			UnityEvents.OnObstacleCollision -= () => PlayCollisionSound(typeof(Obstacle));

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

		private void PlayCollisionSound(Type interactible)
		{
			switch (interactible)
			{
				case var t when t == typeof(Component):
					AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ComponentCollision, this.transform.position);
					break;
				case var t when t == typeof(Collectible):
					AudioManager.Instance.PlayOneShot(FMODEvents.Instance.TrashCollision, this.transform.position);
					break;
				default:
					AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ObstacleCollision, this.transform.position);
					break;
			}
		}
	}
}