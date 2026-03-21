using UnityEngine;

namespace Assets.Settings.GameDefilement
{
	[CreateAssetMenu(fileName = "GameTimerSettings", menuName = "Scriptable Objects/GameTimerSettings")]
	public class GameTimerSettings : ScriptableObject
	{
		[Header("Speed settings")]
		[Tooltip("Base speed in m/s")]
		[Min(0)] public float BaseSpeed;
		[Tooltip("Max speed in m/s")]
		[Min(0)] public float MaxSpeed;

		[Header("Acceleration settings")]
		[Tooltip("Speed increase ratio in m/s")]
		[Min(0)] public float SpeedIncreaseRate;
		[Tooltip("Delay between each speed increase in seconds")]
		[Min(0)] public float SpeedIncreaseDelay;
	}
}