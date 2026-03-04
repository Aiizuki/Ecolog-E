using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference _slideLeftInput;
    [SerializeField] private InputActionReference _slideRightInput;
    [SerializeField] private InputActionReference _slideDownInput;
    [SerializeField] private InputActionReference _jumpInput;

    [Header("Jump parameters")]
    [SerializeField, Tooltip("Duration of jump in seconds")] private float _jumpDuration = 1f;
    [SerializeField] private float _jumpHeight = 2f;
    [SerializeField] private float _speedFallMultiplier = 2f;
    [SerializeField] private AnimationCurve _jumpCurve;
    [SerializeField] private AnimationCurve _fallCurve;

    [Header("Slide parameters")]
    [SerializeField] private float _slideDuration = 1f;
    [SerializeField] private float _slideDown = 1.5f;
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
        _slideLeftInput.action.performed += HandleSlideRight;
        _slideRightInput.action.performed += HandleSlideLeft;
        _slideDownInput.action.performed += HandleSlideDown;
        _jumpInput.action.performed += HandleJump;
    }

    private void Start()
    {
        _baseHeight = transform.position.y;
    }

    private void OnDisable()
    {
        _slideLeftInput.action.performed -= HandleSlideRight;
        _slideRightInput.action.performed -= HandleSlideLeft;
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
            _slideVerticalCoroutine = StartCoroutine(SlideDownCoroutine());
        }
    }

    private void HandleSlideRight(InputAction.CallbackContext context)
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
        _slideHorizontalCoroutine = StartCoroutine(SlideCoroutine(_slideTarget[_currentLaneIndex]));
    }

    private void HandleSlideLeft(InputAction.CallbackContext context)
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
        _slideHorizontalCoroutine = StartCoroutine(SlideCoroutine(_slideTarget[_currentLaneIndex]));
    }

    #endregion Input Actions

    #region Coroutines

    private IEnumerator JumpCoroutine()
    {
        _isJumping = true;
        float jumpTimer = 0f;
        float halfJumpDuration = _jumpDuration / 2f;

        while (jumpTimer < halfJumpDuration)
        {
            jumpTimer += Time.deltaTime;
            float normalizedTime = jumpTimer / halfJumpDuration;
            float targetHeight = _baseHeight + _jumpCurve.Evaluate(normalizedTime) * _jumpHeight;
            transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
            yield return null;
        }

        _fallCoroutine = StartCoroutine(FallCoroutine());
        yield return _fallCoroutine;

        _isJumping = false;
    }

    private IEnumerator FallCoroutine(bool speedFall = false)
    {
        float timer = 0f;
        float halfFallDuration = (_jumpDuration / 2f) / (speedFall ? _speedFallMultiplier : 1f);
        float startHeight = transform.position.y;

        while (timer < halfFallDuration)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / halfFallDuration;
            float targetHeight = _baseHeight + _fallCurve.Evaluate(normalizedTime) * (startHeight - _baseHeight);
            transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, _baseHeight, transform.position.z);
    }

    private IEnumerator SlideDownCoroutine()
    {
        _isSlidingDown = true;
        //_animator.SetBool("IsSlidingDown", true);

        float slideTimer = 0f;

        while (slideTimer <= _slideDown)
        {
            slideTimer += Time.deltaTime;
            yield return null;
        }

        _isSlidingDown = false;
        //_animator.SetBool("IsSlidingDown", false);
    }

    private IEnumerator SlideCoroutine(Transform target)
    {
        _isSlidingHorizontally = true;
        float slideTimer = 0f;

        while (slideTimer < _slideDuration)
        {
            slideTimer += Time.deltaTime;

            float normalizedTime = slideTimer / _slideDuration;
            Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);

            transform.position = Vector3.Lerp(transform.position, targetPosition, normalizedTime);

            yield return null;
        }

        _isSlidingHorizontally = false;
    }

    #endregion Coroutines
}