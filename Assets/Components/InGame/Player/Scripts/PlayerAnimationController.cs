using Assets.Components.Singletons;
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
	}

	private void RevokeEvents()
	{
		UnityEvents.Instance.PlayRunAnimation.RemoveListener(PlayRunAnimation);
		UnityEvents.Instance.PlayDodgeAnimation.RemoveListener(PlayDodgeAnimation);
		UnityEvents.Instance.PlayDeathAnimation.RemoveListener(PlayDeathAnimation);
		UnityEvents.Instance.PlayJumpAnimation.RemoveListener(PlayJumpAnimation);
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

	#endregion Unity Events
}
