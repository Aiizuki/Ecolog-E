using Assets.Components.Singletons;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Components.UI
{
	/// <summary>
	/// Handles scene transitions with fade effects, including loading new scenes, reloading the current scene, and transitioning to game over or win screens
	/// </summary>
	public class SceneTransitionManager : MonoBehaviour
	{
		[SerializeField] private CanvasGroup fadeCanvasGroup;
		[SerializeField] private float fadeDuration = 0.5f;
		[SerializeField] private float blackScreenDuration = 0.3f;

		private void Start()
		{
			UnityEvents.Instance.ReturnToHomeEvent.AddListener(OnReturnToHome);
			UnityEvents.Instance.NewGameEvent.AddListener(OnNewGame);
			UnityEvents.Instance.GameOverEvent.AddListener(OnGameOver);
		}

		public void ReloadCurrentScene()
		{
			StartCoroutine(ReloadSceneRoutine());
		}

		private IEnumerator TransitionToScene(string sceneName)
		{
			yield return StartCoroutine(FadeToBlack());
			yield return new WaitForSeconds(blackScreenDuration);

			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
			while (!asyncLoad.isDone)
				yield return null;

			yield return StartCoroutine(FadeFromBlack());
		}

		private IEnumerator NewGameRoutine()
			=> TransitionToScene("GameScene");

		private IEnumerator ReturnToHomeRoutine()
			=> TransitionToScene("HomeScene");

		private IEnumerator GameWinRoutine()
			=> TransitionToScene("GameWin");

		private IEnumerator LoadGameOverSceneRoutine()
			=> TransitionToScene("GameOver");

		/// <summary>
		/// Reloads the current scene with a fade to black and fade from black effect, ensuring that all sounds are cleaned up before reloading the scene*
		/// Used when the player go to the next floor
		/// </summary>
		private IEnumerator ReloadSceneRoutine()
		{
			int sceneIndex = SceneManager.GetActiveScene().buildIndex;

			yield return StartCoroutine(FadeToBlack());

			yield return new WaitForSeconds(blackScreenDuration);

			//AudioManager.Instance.CleanSounds();
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
			while (!asyncLoad.isDone)
				yield return null;

			yield return StartCoroutine(FadeFromBlack());
		}

		/// <summary>
		/// Fades the screen to black over the specified duration
		/// </summary>
		/// <returns></returns>
		private IEnumerator FadeToBlack()
		{
			float elapsedTime = 0f;

			while (elapsedTime < fadeDuration)
			{
				elapsedTime += Time.unscaledDeltaTime;
				fadeCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
				yield return null;
			}

			fadeCanvasGroup.alpha = 1f;
		}

		/// <summary>
		/// Fades the screen from black to transparent over the specified duration
		/// </summary>
		/// <returns></returns>
		private IEnumerator FadeFromBlack()
		{
			float elapsedTime = 0f;

			while (elapsedTime < fadeDuration)
			{
				elapsedTime += Time.unscaledDeltaTime;
				fadeCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
				yield return null;
			}

			fadeCanvasGroup.alpha = 0f;
		}

		#region Event Listeners

		private void OnReturnToHome()
			=> StartCoroutine(ReturnToHomeRoutine());

		private void OnNewGame()
			=> StartCoroutine(NewGameRoutine());

		private void OnGameWin()
			=> StartCoroutine(GameWinRoutine());

		private void OnGameOver()
			=> StartCoroutine(LoadGameOverSceneRoutine());

		#endregion Event Listeners
	}
}