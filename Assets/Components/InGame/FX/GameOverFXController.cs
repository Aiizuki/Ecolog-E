using Assets.Components.Singletons;
using System.Collections;
using UnityEngine;

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
		UnityEvents.Instance.GameOverEvent.AddListener(PlayGameOverFX);
		UnityEvents.Instance.NotifyDeathAnimationFinishedEvent.AddListener(ChangeState);
	}

	private void RevokeEvents()
	{
		UnityEvents.Instance.GameOverEvent.RemoveListener(PlayGameOverFX);
		UnityEvents.Instance.NotifyDeathAnimationFinishedEvent.RemoveListener(ChangeState);
	}

	#endregion Unity Events

	private void PlayGameOverFX()
	{
		StartCoroutine(GameOver());
	}

	private void ChangeState()
		=> _deathAnimationDone = true;

	private IEnumerator GameOver()
	{
		yield return StartCoroutine(PlayPlayerDeathAnimation());
		yield return StartCoroutine(PlayCameraAnimation());
	}

	private IEnumerator PlayPlayerDeathAnimation()
	{
		UnityEvents.Instance.PlayDeathAnimation.Invoke();
		yield return new WaitUntil(() => _deathAnimationDone);
	}

	private IEnumerator PlayCameraAnimation()
	{
		UnityEvents.Instance.GameOverTransitionEvent.Invoke();
		yield return null;
	}
}
