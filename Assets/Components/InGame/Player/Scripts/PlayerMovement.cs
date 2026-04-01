using Assets.Components.Singletons;
using Assets.Settings.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private PlayerConfig _playerConfig;

	[Header("Inputs")]
	[SerializeField] private InputActionReference _slideLeftInput;
	[SerializeField] private InputActionReference _slideRightInput;
	[SerializeField] private InputActionReference _slideDownInput;
	[SerializeField] private InputActionReference _jumpInput;

	[Header("Jump parameters")]
	[SerializeField] private AnimationCurve _jumpCurve;
	[SerializeField] private AnimationCurve _fallCurve;

	[Header("Slide parameters")]
	[SerializeField] private Transform[] _slideTarget;

	[Header("Components")]
	[SerializeField] private Animator _animator;

	[Header("Debug")]
	[SerializeField] private int _currentLaneIndex = 1;
	[SerializeField] private bool _isSlidingHorizontally;
	[SerializeField] private bool _isSlidingDown;
	[SerializeField] private bool _isJumping;

	private float _baseHeight;
	private Coroutine _slideHorizontalCoroutine;
	private Coroutine _slideVerticalCoroutine;
	private Coroutine _fallCoroutine;

	#region Unity Lifecycle

	private void OnEnable()
	{
		_slideLeftInput.action.performed += HandleSlideLeft;
		_slideRightInput.action.performed += HandleSlideRight;
		_slideDownInput.action.performed += HandleSlideDown;
		_jumpInput.action.performed += HandleJump;
	}

	private void Start()
	{
		_baseHeight = transform.position.y;
	}

	private void OnDisable()
	{
		_slideLeftInput.action.performed -= HandleSlideLeft;
		_slideRightInput.action.performed -= HandleSlideRight;
		_slideDownInput.action.performed -= HandleSlideDown;
		_jumpInput.action.performed -= HandleJump;
	}

	#endregion Unity Lifecycle

	#region Inputs Actions

	private void HandleJump(InputAction.CallbackContext context)
	{
		if (_isJumping)
		{
			return;
		}
		else if (_isSlidingDown)
		{
			StopCoroutine(_slideVerticalCoroutine);
			_isSlidingDown = false;
		}

		_slideVerticalCoroutine = StartCoroutine(JumpCoroutine());
	}

	private void HandleSlideDown(InputAction.CallbackContext context)
	{
		if (_isSlidingDown)
			return;

		if (_isJumping)
		{
			// Stop les deux coroutines actives
			StopCoroutine(_slideVerticalCoroutine);

			if (_fallCoroutine != null)
				StopCoroutine(_fallCoroutine);

			_isJumping = false;
			_fallCoroutine = StartCoroutine(FallCoroutine(speedFall: true));
		}
		else
		{
			_slideVerticalCoroutine = StartCoroutine(CrouchCoroutine());
		}
	}

	private void HandleSlideLeft(InputAction.CallbackContext context)
	{
		if (_isSlidingHorizontally)
		{
			StopCoroutine(_slideHorizontalCoroutine);
			_isSlidingHorizontally = false;
		}

		if (_currentLaneIndex == 0)
		{
			return;
		}

		_currentLaneIndex--;
		UnityEvents.Instance.PlayDodgeAnimation.Invoke(true);
		_slideHorizontalCoroutine = StartCoroutine(StrafeCoroutine(_slideTarget[_currentLaneIndex]));
	}

	private void HandleSlideRight(InputAction.CallbackContext context)
	{
		if (_isSlidingHorizontally)
		{
			StopCoroutine(_slideHorizontalCoroutine);
			_isSlidingHorizontally = false;
		}

		if (_currentLaneIndex == _slideTarget.Length - 1)
		{
			return;
		}

		_currentLaneIndex++;
		UnityEvents.Instance.PlayDodgeAnimation.Invoke(false);
		_slideHorizontalCoroutine = StartCoroutine(StrafeCoroutine(_slideTarget[_currentLaneIndex]));
	}

	#endregion Input Actions

	#region Coroutines

	private IEnumerator JumpCoroutine()
	{
		_isJumping = true;
		UnityEvents.Instance.PlayJumpAnimation.Invoke();
		_animator.SetBool("IsGrounded", false);

		float jumpTimer = 0f;

		while (jumpTimer < 0.5f)
		{
			jumpTimer += Time.deltaTime;
			float normalizedTime = jumpTimer;
			float targetHeight = _baseHeight + _jumpCurve.Evaluate(normalizedTime) * _playerConfig.JumpHeight;
			transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
			yield return null;
		}

		yield return new WaitForSeconds(_playerConfig.JumpFloatingTime);
		_fallCoroutine = StartCoroutine(FallCoroutine());
		yield return _fallCoroutine;

		_animator.SetBool("IsGrounded", true);
		_isJumping = false;
	}

	private IEnumerator FallCoroutine(bool speedFall = false)
	{
		float timer = 0f;
		float startHeight = transform.position.y;

		while (timer < 0.5f)
		{
			timer += Time.deltaTime;
			float normalizedTime = timer;
			float targetHeight = _baseHeight + _fallCurve.Evaluate(normalizedTime) * (startHeight - _baseHeight);
			transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
			yield return null;
		}

		transform.position = new Vector3(transform.position.x, _baseHeight, transform.position.z);
	}

	private IEnumerator CrouchCoroutine()
	{
		_isSlidingDown = true;
		_animator.SetBool("Crouch", true);

		UnityEvents.Instance.PlayCrouchAnimation.Invoke();
		yield return new WaitForSeconds(_playerConfig.CrouchDurantion);

		_isSlidingDown = false;
		_animator.SetBool("Crouch", false);
	}

	private IEnumerator StrafeCoroutine(Transform target)
	{
		_isSlidingHorizontally = true;
		float slideTimer = 0f;

		while (slideTimer < _playerConfig.StrafeDuration)
		{
			slideTimer += Time.deltaTime;

			float normalizedTime = slideTimer / _playerConfig.StrafeDuration;
			Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);

			transform.position = Vector3.Lerp(transform.position, targetPosition, normalizedTime);

			yield return null;
		}

		_isSlidingHorizontally = false;
	}

	#endregion Coroutines
}