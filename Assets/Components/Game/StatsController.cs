using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Components.Game
{
	public class StatsController : MonoBehaviour
	{
		public static int Score;

		private void Start()
		{
			UnityEvents.Instance.NewGameEvent.AddListener(ResetStats);
		}

		#region Static Helpers

		public static void SetScore(int score)
			=> Score = score;

		public static int GetScore()
			=> Score;

		#endregion Static Helpers

		#region Private Methods

		private void ResetStats()
		{
			Score = 0;
		}

		#endregion Private Methods
	}
}
