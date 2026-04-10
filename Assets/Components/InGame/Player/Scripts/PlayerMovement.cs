using Assets.Components.Singletons;
using Assets.Settings.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Components.InGame.Player.Scripts
{
	public class PlayerMovement : MonoBehaviour
	{
		[SerializeField] private PlayerConfig _playerConfig;

		[Header("Jump parameters")]
		[SerializeField] private AnimationCurve _jumpCurve;
		[SerializeField] private AnimationCurve _fallCurve;

		[Header("Slide parameters")]
		[SerializeField] private Transform[] _slideTarget;

		[Header("Components")]
		[SerializeField] private Animator _animator;

		[Header("Debug")]
		[SerializeField] private int _currentLaneIndex = 1;
		[SerializeField] private bool _isStrafing;
		[SerializeField] private bool _isCrouching;
		[SerializeField] private bool _isJumping;
		private bool _canFall = true;

		private float _baseHeight;
		private Coroutine _strafeCoroutine;
		private Coroutine _jumpCoroutine;
		private Coroutine _fallCoroutine;

		private InputBuffer _inputBuffer;
		private const string JUMP_INPUT_NAME = "Jump";
		private const string STRAFE_LEFT_INPUT_NAME = "SlideLeft";
		private const string STRAFE_RIGHT_INPUT_NAME = "SlideRight";
		private const string CROUCH_INPUT_NAME = "Crouch";

		#region Unity Lifecycle

		private void Start()
		{
			_inputBuffer ??= new InputBuffer();
			_baseHeight = transform.position.y;
		}

		#endregion Unity Lifecycle

		#region Inputs Actions

		public void OnJump(InputAction.CallbackContext context)
		{
			_inputBuffer.Buffer(JUMP_INPUT_NAME);

			if (_isJumping)
				return;
			if (!_inputBuffer.TryConsume(JUMP_INPUT_NAME))
				return;

			if (_isCrouching)
			{
				StopCoroutine(_jumpCoroutine);
				_isCrouching = false;
			}

			_jumpCoroutine = StartCoroutine(JumpCoroutine());
		}

		public void OnCrouch(InputAction.CallbackContext context)
		{
			_inputBuffer.Buffer(CROUCH_INPUT_NAME);

			if (_isCrouching)
				return;
			if (!_inputBuffer.TryConsume(CROUCH_INPUT_NAME))
				return;

			if (_isJumping)
			{
				// Stop les deux coroutines actives
				StopCoroutine(_jumpCoroutine);
				if (_canFall)
				{
					if (_fallCoroutine != null)
						StopCoroutine(_fallCoroutine);

					_fallCoroutine = StartCoroutine(FallCoroutine(speedFall: true));
				}
			}
			else
			{
				_jumpCoroutine = StartCoroutine(CrouchCoroutine());
			}
		}

		public void OnSlideLeft(InputAction.CallbackContext context)
		{
			_inputBuffer.Buffer(STRAFE_LEFT_INPUT_NAME);

			if (!_inputBuffer.TryConsume(STRAFE_LEFT_INPUT_NAME))
				return;

			if (_isStrafing)
			{
				StopCoroutine(_strafeCoroutine);
				_isStrafing = false;
			}

			if (_currentLaneIndex == 0)
			{
				return;
			}

			_currentLaneIndex--;
			UnityEvents.PlayDodgeAnimation.Invoke(true);
			_strafeCoroutine = StartCoroutine(StrafeCoroutine(_slideTarget[_currentLaneIndex]));
		}

		public void OnSlideRight(InputAction.CallbackContext context)
		{
			_inputBuffer.Buffer(STRAFE_RIGHT_INPUT_NAME);

			if (!_inputBuffer.TryConsume(STRAFE_RIGHT_INPUT_NAME))
				return;

			if (_isStrafing)
			{
				StopCoroutine(_strafeCoroutine);
				_isStrafing = false;
			}

			if (_currentLaneIndex == _slideTarget.Length - 1)
			{
				return;
			}

			_currentLaneIndex++;
			UnityEvents.PlayDodgeAnimation.Invoke(false);
			_strafeCoroutine = StartCoroutine(StrafeCoroutine(_slideTarget[_currentLaneIndex]));
		}

		#endregion Input Actions

		#region Coroutines

		private IEnumerator JumpCoroutine()
		{
			_isJumping = true;
			UnityEvents.PlayJumpAnimation.Invoke();

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
		}

		private IEnumerator FallCoroutine(bool speedFall = false)
		{
			_canFall = false;
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
			_animator.SetBool("IsGrounded", true);
			_isJumping = false;
			_canFall = true;
		}

		private IEnumerator CrouchCoroutine()
		{
			_isCrouching = true;
			_animator.SetBool("IsCrouching", _isCrouching);
			UnityEvents.PlayCrouchAnimation.Invoke();
			yield return new WaitForSeconds(_playerConfig.CrouchDurantion);
			_isCrouching = false;
			_animator.SetBool("IsCrouching", _isCrouching);
		}

		private IEnumerator StrafeCoroutine(Transform target)
		{
			_isStrafing = true;
			float slideTimer = 0f;

			while (slideTimer < _playerConfig.StrafeDuration)
			{
				slideTimer += Time.deltaTime;

				float normalizedTime = slideTimer / _playerConfig.StrafeDuration;
				Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);

				transform.position = Vector3.Lerp(transform.position, targetPosition, normalizedTime);

				yield return null;
			}

			_isStrafing = false;
		}

		#endregion Coroutines
	}
}