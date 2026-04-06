using Assets.Components.Singletons;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Components.InGame.Player.Scripts
{
	public class PlayerAnimationController : MonoBehaviour
	{
		[SerializeField] private Animator _animator;

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
			UnityEvents.PlayRunAnimation += OnPlayRunAnimation;
			UnityEvents.PlayDodgeAnimation += OnPlayDodgeAnimation;
			UnityEvents.PlayDeathAnimation += OnPlayDeathAnimation;
			UnityEvents.PlayJumpAnimation += OnPlayJumpAnimation;
			UnityEvents.PlayCrouchAnimation += OnPlayCrouchAnimation;
			UnityEvents.SpeedIncreaseEvent += OnSpeedIncrease;
			UnityEvents.HealthLoose += OnHealthLoose;
		}

		private void RevokeEvents()
		{
			UnityEvents.PlayRunAnimation -= OnPlayRunAnimation;
			UnityEvents.PlayDodgeAnimation -= OnPlayDodgeAnimation;
			UnityEvents.PlayDeathAnimation -= OnPlayDeathAnimation;
			UnityEvents.PlayJumpAnimation -= OnPlayJumpAnimation;
			UnityEvents.PlayCrouchAnimation -= OnPlayCrouchAnimation;
			UnityEvents.SpeedIncreaseEvent -= OnSpeedIncrease;
			UnityEvents.HealthLoose -= OnHealthLoose;
		}

		private void OnPlayRunAnimation()
			=> _animator.SetBool("Running", true);

		private void OnPlayDodgeAnimation(bool isLeft)
		{
			if (isLeft)
				_animator.SetTrigger("StrafeLeft");
			else
				_animator.SetTrigger("StrafeRight");
		}

		private void OnPlayDeathAnimation()
		{
			_animator.SetBool("IsDead", true);
			_animator.SetTrigger("Dead");
		}

		private void OnPlayJumpAnimation()
		{
			_animator.SetTrigger("Jump");
			_animator.SetBool("IsGrounded", false);
		}

		private void OnPlayCrouchAnimation()
			=> _animator.SetTrigger("Crouch");

		private void OnSpeedIncrease(float speed)
			=> _animator.SetFloat("Speed", speed);

		private void OnHealthLoose(int? _)
		{
			float random = RandomisationHelper.RandomChooseBoolean() ? 1f : 0f;
			_animator.SetFloat("HitRandomizer", random);
			_animator.SetTrigger("Hit");
		}

		#endregion Unity Events

		/// <summary>
		/// Called in the animator when the death animation is complete (in A_Pose)
		/// </summary>
		public void FireDeathAnimationDone()
			=> UnityEvents.NotifyDeathAnimationFinishedEvent.Invoke();
	}
}