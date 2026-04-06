using Assets.Components.SaveService;
using Assets.Components.SaveService.Components.SaveService;
using Assets.Components.Singletons;
using UnityEngine;

namespace Assets.Components.Game
{
	public class StatsController : MonoBehaviour
	{
		public static int Score;
		public static int InGameTime;

		private SaveData _saveData;

		private void Start()
		{
			_saveData = SaveServiceController.Load();
			UnityEvents.Instance.NewGameEvent.AddListener(ResetStats);
			UnityEvents.Instance.GameOverEvent.AddListener(SaveStats);
		}

		#region Static Helpers

		public static void AddScore(int score)
		{
			Score += score;
			UnityEvents.Instance.ScoreUpdateEvent.Invoke(Score);
		}

		#endregion Static Helpers

		#region Private Methods

		private void ResetStats()
		{
			Score = 0;
		}

		private void SaveStats()
		{
			_saveData.Score = Score;
			_saveData.RunCount++;

			SaveServiceController.Save(_saveData);
		}

		#endregion Private Methods
	}
}
