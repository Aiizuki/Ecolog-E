using Assets.Components.SaveService;
using Assets.Components.SaveService.Components.SaveService;
using Assets.Components.UI.FX;
using TMPro;
using UnityEngine;

namespace Assets.Components.UI.Stats
{
	public class StatsDisplayer : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _runNumberText;

		[SerializeField] private TextMeshProUGUI _scoreText;
		[SerializeField] private NewRecordFX _newRecordScoreFX;

		[SerializeField] private TextMeshProUGUI _gameTimeText;
		[SerializeField] private NewRecordFX _newRecordGameTimeFX;

		[SerializeField] private TextMeshProUGUI _trashCollectedText;
		[SerializeField] private NewRecordFX _newRecordTrashCollectedFX;

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
			_scoreText.text = data.RunScore.ToString();
			_gameTimeText.text = data.LastRunTime.ToString();
			_trashCollectedText.text = data.TrashCollected.ToString();
			_moneyCollectedText.text = data.ComponentsCollected.ToString();

			_newRecordScoreFX.gameObject.SetActive(data.RunScore >= data.PlayerBestScore);
			_newRecordGameTimeFX.gameObject.SetActive(data.LastRunTime >= data.PlayerBestRunTime);
			_newRecordTrashCollectedFX.gameObject.SetActive(data.TrashCollected >= data.PlayerBestTrashCollected);
		}

		private void OnDestroy()
		{
			_newRecordScoreFX.gameObject.SetActive(false);
			_newRecordGameTimeFX.gameObject.SetActive(false);
			_newRecordTrashCollectedFX.gameObject.SetActive(false);
		}
	}
}