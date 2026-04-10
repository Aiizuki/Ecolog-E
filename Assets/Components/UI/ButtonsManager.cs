using Assets.Components.Audio;
using Assets.Components.Singletons;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsManager : MonoBehaviour
{
	#region Unity Lifecycle

	protected void OnEnable()
	{
		this.GetComponent<Button>().interactable = true;
	}

	protected void OnDisable()
	{
		StopAllCoroutines();
	}

	#endregion Unity Lifecycle

	public void FireReturnToHomeEvent()
	{
		PlayButtonClick();
		UnityEvents.ReturnToHome.Invoke();
	}

	public void FireNewGameEvent()
	{
		PlayButtonClick();
		UnityEvents.Instance.NewGame.Invoke();
	}

	public void PlayButtonClick(bool preventSpam = true)
	{
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ButtonClick, Vector2.zero);
		if (preventSpam)
			StartCoroutine(PreventSpamClickRoutine());
	}

	private IEnumerator PreventSpamClickRoutine()
	{
		this.GetComponent<Button>().interactable = false;
		yield return new WaitForSeconds(1f);
		this.GetComponent<Button>().interactable = true;
	}
}
