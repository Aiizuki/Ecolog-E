using Assets.Components.Singletons;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsManager : MonoBehaviour
{
	public void FireReturnToHomeEvent()
	{
		PlayButtonClickSound();
		UnityEvents.ReturnToHome.Invoke();
	}

	public void FireNewGameEvent()
	{
		PlayButtonClickSound();
		UnityEvents.Instance.NewGame.Invoke();
	}

	public void PlayButtonClickSound()
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
