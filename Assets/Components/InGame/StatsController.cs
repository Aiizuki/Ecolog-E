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
			UnityEvents.Instance.NewGame.AddListener(OnNewGame);
			UnityEvents.Instance.GameOver.AddListener(OnGameOver);
		}

		#region Static Helpers

		public static void AddScore(int score)
		{
			Score += score;
			UnityEvents.ScoreUpdate.Invoke(Score);
		}

		#endregion Static Helpers

		#region Private Methods

		private void OnNewGame()
		{
			Score = 0;
		}

		private void OnGameOver()
		{
			_saveData.Score = Score;
			_saveData.RunCount++;

			SaveServiceController.Save(_saveData);
		}

		#endregion Private Methods
	}
}
