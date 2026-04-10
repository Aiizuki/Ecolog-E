using FMODUnity;
using UnityEngine;

namespace Assets.Components.Audio
{
	/// <summary>
	/// Represents a collection of FMOD sounds / params used in the game. 
	/// </summary>
	public class FMODEvents : MonoBehaviour
	{
		[Header("Parameters")]
		public const string GameOverEvent = "GameOver";

		[Header("UI sounds")]
		[field: SerializeField] public EventReference ButtonClick { get; private set; }
		[field: SerializeField] public EventReference NotificationFailure { get; private set; }

		[Header("Player sounds")]
		[field: SerializeField] public EventReference ObstacleCollision { get; private set; }
		[field: SerializeField] public EventReference TrashCollision { get; private set; }
		[field: SerializeField] public EventReference ComponentCollision { get; private set; }
		[Tooltip("Regroup strafe/jump and fall")][field: SerializeField] public EventReference PlayerMovement { get; private set; }
		[field: SerializeField] public EventReference Crouch { get; private set; }


		public static FMODEvents Instance { get; private set; }

		private void Awake()
		{
			if (Instance != null)
				return;

			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}
}