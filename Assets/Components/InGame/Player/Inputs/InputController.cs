using Assets.Components.InGame.Player.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
	[Header("Inputs")]
	[SerializeField] private InputActionReference _slideLeftInput;
	[SerializeField] private InputActionReference _slideRightInput;
	[SerializeField] private InputActionReference _crouchInput;
	[SerializeField] private InputActionReference _jumpInput;

	[SerializeField] private PlayerMovement _playerMovement;

	#region Unity Lifecycle

	void OnEnable()
	{
		InitEvents();
	}

	private void OnDisable()
	{
		RevokeEvents();
	}

	#endregion Unity Lifecycle

	#region Unity Events

	private void InitEvents()
	{
		_slideLeftInput.action.performed += _playerMovement.OnSlideLeft;
		_slideRightInput.action.performed += _playerMovement.OnSlideRight;
		_crouchInput.action.performed += _playerMovement.OnCrouch;
		_jumpInput.action.performed += _playerMovement.OnJump;
	}

	private void RevokeEvents()
	{
		_slideLeftInput.action.performed -= _playerMovement.OnSlideLeft;
		_slideRightInput.action.performed -= _playerMovement.OnSlideRight;
		_crouchInput.action.performed -= _playerMovement.OnCrouch;
		_jumpInput.action.performed -= _playerMovement.OnJump;
	}

	#endregion Unity Events
}
