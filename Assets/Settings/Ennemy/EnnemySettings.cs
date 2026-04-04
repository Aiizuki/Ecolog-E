using UnityEngine;

namespace Assets.Settings.GameDefilement
{
	[CreateAssetMenu(fileName = "EnnemySettings", menuName = "Scriptable Objects/EnnemySettings")]
	public class EnnemySettings : ScriptableObject
	{
		[Header("Ennemy Settings")]
		[Tooltip("Time (in seconds) before the ennemy throws a new projectile")] public float ThrowingInterval;

		[Header("ProjectileSettings")]
		[Tooltip("Lifetime of the projectile (in seconds)")] public float ProjectileLifetime = 5f;
		[Tooltip("Speed of the projectile (in m/s)")] public float ProjectileSpeed = 1f;
	}
}