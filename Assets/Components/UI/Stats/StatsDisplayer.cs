using Assets.Components.SaveService;
using Assets.Components.SaveService.Components.SaveService;
using TMPro;
using UnityEngine;

namespace Assets.Components.UI.Stats
{
	public class StatsDisplayer : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _runNumberText;
		[SerializeField] private TextMeshProUGUI _scoreText;
		[SerializeField] private TextMeshProUGUI _gameTimeText;
		[SerializeField] private TextMeshProUGUI _trashCollectedText;
		[SerializeField] private TextMeshProUGUI _moneyCollectedText;

		private void Start()
		{
			SaveData data = SaveServiceController.Load();

			if (data == null)
			{
				Debug.LogWarning("There is no save file !");
				return;
			}

			_runNumberText.text = data.RunCount.ToString();
			_scoreText.text = data.Score.ToString();
			_gameTimeText.text = data.LastRunTime.ToString();
			_trashCollectedText.text = data.TrashCollected.ToString();
			_moneyCollectedText.text = data.MoneyEarned.ToString();
		}
	}
}