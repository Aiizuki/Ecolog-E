using Assets.Components.Singletons;
using Assets.Scripts.Helpers;
using UnityEngine;

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
		UnityEvents.Instance.PlayRunAnimation.AddListener(PlayRunAnimation);
		UnityEvents.Instance.PlayDodgeAnimation.AddListener(PlayDodgeAnimation);
		UnityEvents.Instance.PlayDeathAnimation.AddListener(PlayDeathAnimation);
		UnityEvents.Instance.PlayJumpAnimation.AddListener(PlayJumpAnimation);
		UnityEvents.Instance.PlayCrouchAnimation.AddListener(PlaySlideAnimation);
		UnityEvents.Instance.SpeedIncreaseEvent.AddListener(ChangeSpeedAnimation);
		UnityEvents.Instance.HealthLooseEvent.AddListener(PlayHitAnimation);
	}

	private void RevokeEvents()
	{
		UnityEvents.Instance.PlayRunAnimation.RemoveListener(PlayRunAnimation);
		UnityEvents.Instance.PlayDodgeAnimation.RemoveListener(PlayDodgeAnimation);
		UnityEvents.Instance.PlayDeathAnimation.RemoveListener(PlayDeathAnimation);
		UnityEvents.Instance.PlayJumpAnimation.RemoveListener(PlayJumpAnimation);
		UnityEvents.Instance.PlayCrouchAnimation.RemoveListener(PlaySlideAnimation);
		UnityEvents.Instance.SpeedIncreaseEvent.RemoveListener(ChangeSpeedAnimation);
		UnityEvents.Instance.HealthLooseEvent.RemoveListener(PlayHitAnimation);
	}

	private void PlayRunAnimation()
		=> _animator.SetBool("Running", true);

	private void PlayDodgeAnimation(bool isLeft)
	{
		if (isLeft)
			_animator.SetTrigger("StrafeLeft");
		else
			_animator.SetTrigger("StrafeRight");
	}

	private void PlayDeathAnimation()
		=> _animator.SetTrigger("Dead");

	private void PlayJumpAnimation()
		=> _animator.SetTrigger("Jump");

	private void PlaySlideAnimation()
		=> _animator.SetTrigger("Slide");

	private void ChangeSpeedAnimation(float speed)
		=> _animator.SetFloat("Speed", speed);

	private void PlayHitAnimation(int? _)
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
		=> UnityEvents.Instance.NotifyDeathAnimationFinishedEvent.Invoke();
}
