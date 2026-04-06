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
			UnityEvents.ScoreUpdate += OnScoreUpdate;
		}

		private void RevokeEvents()
		{
			UnityEvents.ScoreUpdate -= OnScoreUpdate;
		}

		#endregion Unity Events

		private void OnScoreUpdate(int score)
		{
			_scoreText.text = score.ToString();
		}
	}
}
