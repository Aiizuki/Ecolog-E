using Assets.Scripts.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsManager : MonoBehaviour
{
	public void FireReturnToHomeEvent()
	{
		PlayButtonClickSound();
		UnityEvents.Instance.ReturnToHomeEvent.Invoke();
	}

	public void FireNewGameEvent()
	{
		PlayButtonClickSound();
		UnityEvents.Instance.NewGameEvent.Invoke();
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
