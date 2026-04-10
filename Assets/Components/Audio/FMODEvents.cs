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