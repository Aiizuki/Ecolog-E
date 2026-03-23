using Assets.Components.Singletons;
using TMPro;
using UnityEngine;

namespace Assets.Components.UI.Game
{
	public class ScoreUpdater : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _scoreText;

		#region Unity Lifecycle

		private void Start()
		{
			InitEvents();
		}

		private void OnDestroy()
		{
			RevokeEvents();
		}

		#endregion Unity Lifecycle

		#region Unity Events

		private void InitEvents()
		{
			UnityEvents.Instance.ScoreUpdateEvent.AddListener(UpdateScoreText);
		}

		private void RevokeEvents()
		{
			UnityEvents.Instance.ScoreUpdateEvent.RemoveListener(UpdateScoreText);
		}

		#endregion Unity Events

		private void UpdateScoreText(int score)
		{
			_scoreText.text = score.ToString();
		}
	}
}
