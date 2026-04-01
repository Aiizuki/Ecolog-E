using System.Collections.Generic;
using UnityEngine;

namespace Assets.Settings.Player
{
	[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Scriptable Objects/PlayerConfig")]
	public class PlayerConfig : ScriptableObject
	{
		[Header("Health")]
		[Min(0)] public int MaxHealth;
		[Min(0)] public int MinHealth;

		[Tooltip("How many health the player loose each tick (defined in HealthLooseRate)")]
		public float HealthLooseRatio;
		[Tooltip("Frequency of health loss (in seconds)")]
		[Min(1)] public float HealthLooseRate;

		[Tooltip("How many health the player recover when collecting a trash")]
		public int HealthGainPerTrashCollect;

		[Header("Collisions Rules")]
		[Min(0)] public float InvincibilityDuration;

		[Tooltip("List of each key distances, after each key the damages are increased")]
		public List<int> lstDistanceCheckpoints;
		public List<int> lstDamagePerCheckpoints;

		[Header("Movement")]

		[Header("Jump parameters")]
		[Tooltip("In-air time (in seconds)")] public float JumpFloatingTime = 1f;
		[Tooltip("Jump Height (in meters)")] public float JumpHeight = 2f;
		[Tooltip("Speed fall multiplier")] public float SpeedFallMultiplier = 2f;

		[Header("Slide parameters")]
		[Tooltip("Strafe duration (in seconds)")] public float StrafeDuration = 0.12f;
		[Tooltip("Crouch duration (in seconds)")] public float CrouchDurantion = 1.5f;
	}
}