using Assets.Components.Singletons;
using System.Collections;
using UnityEngine;

namespace Assets.Components.InGame.FX
{
	public class GameOverFXController : MonoBehaviour
	{
		private bool _deathAnimationDone = false;

		#region Unity Lifecycle

		void Start()
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
			UnityEvents.Instance.GameOver.AddListener(OnGameOver);
			UnityEvents.NotifyDeathAnimationFinishedEvent += OnStateChanged;
		}

		private void RevokeEvents()
		{
			UnityEvents.Instance.GameOver.RemoveListener(OnGameOver);
			UnityEvents.NotifyDeathAnimationFinishedEvent -= OnStateChanged;
		}

		#endregion Unity Events

		private void OnGameOver()
		{
			StartCoroutine(GameOver());
		}

		private void OnStateChanged()
			=> _deathAnimationDone = true;

		private IEnumerator GameOver()
		{
			yield return StartCoroutine(PlayPlayerDeathAnimation());
			UnityEvents.GameOverTransition.Invoke();
		}

		private IEnumerator PlayPlayerDeathAnimation()
		{
			UnityEvents.PlayDeathAnimation.Invoke();
			yield return new WaitUntil(() => _deathAnimationDone);
		}
	}
}