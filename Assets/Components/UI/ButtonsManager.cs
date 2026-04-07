using Assets.Components.Singletons;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsManager : MonoBehaviour
{
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

	public void PlayButtonClick()
	{
		//AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ButtonClick, Vector2.zero);
		StartCoroutine(PreventSpamClickRoutine());
	}

	private IEnumerator PreventSpamClickRoutine()
	{
		this.GetComponent<Button>().interactable = false;
		yield return new WaitForSeconds(1f);
		this.GetComponent<Button>().interactable = true;
	}
}
