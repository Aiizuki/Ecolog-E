using Assets.Components.Audio;
using Assets.Components.SaveService;
using Assets.Components.SaveService.Components.SaveService;
using FMODUnity;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Components.Options
{
	/// <summary>
	/// Handles the options menu, allowing players to adjust audio settings for different banks (Music, SFX, Ambience, UI) and save their preferences using PlayerPrefs. 
	/// </summary>
	public class OptionsManager : MonoBehaviour
	{
		[Header("UI References")]
		public Button closeButton;
		public Transform bankControlsContainer;

		[Header("Prefab")]
		public GameObject bankControlPrefab;

		private bool controlsCreated = false;
		private bool busesInitialized = false;

		private SaveData _saveData;

		#region Unity Lifecycle

		private void Start()
		{
			_saveData = SaveServiceController.Load();
			closeButton.onClick.AddListener(CloseSettings);
			StartCoroutine(InitializeBuses());
		}

		private void OnEnable()
		{
			if (!busesInitialized)
				return;

			if (!controlsCreated)
			{
				CreateBankControls();
				controlsCreated = true;
			}
			else
			{
				UpdateSliderValues();
			}
		}

		#endregion Unity Lifecycle

		/// <summary>
		/// Wait until FMOD is ready, initialize the buses, and then create the UI controls.
		/// </summary>
		private IEnumerator InitializeBuses()
		{
			while (!RuntimeManager.HaveAllBanksLoaded)
			{
				yield return null;
			}

			// Initialiser chaque bus avec gestion d'erreur
			foreach (BankVolumeControl bank in AudioManager.Instance.Banks)
			{
				try
				{
					bank.bus = RuntimeManager.GetBus(bank.busPath);
				}
				catch (BusNotFoundException)
				{
					Debug.LogWarning($"[OptionsManager] Bus not found : '{bank.busPath}'. Checks its path in FMOD Studio");
				}
			}

			busesInitialized = true;
			if (!controlsCreated)
			{
				CreateBankControls();
				controlsCreated = true;
			}
		}

		/// <summary>
		/// Initializes the UI controls for each audio bank, setting their initial values based on saved preferences or current bus volumes.
		/// </summary>
		private void CreateBankControls()
		{
			foreach (BankVolumeControl bank in AudioManager.Instance.Banks)
			{
				if (!bank.bus.isValid())
				{
					Debug.LogWarning($"[OptionsManager] Invalid Bud (ignored) : '{bank.busPath}'");
					continue;
				}

				GameObject control = Instantiate(bankControlPrefab, bankControlsContainer);

				TextMeshProUGUI nameText = control.transform.Find("BankName").GetComponent<TextMeshProUGUI>();
				nameText.text = bank.bankName;

				Slider volumeSlider = control.transform.Find("VolumeSlider").GetComponent<Slider>();

				float savedVolume = _saveData.LstSoundSettings.ContainsKey($"Volume_{bank.bankName}") ? _saveData.LstSoundSettings[$"Volume_{bank.bankName}"] : -1f;
				if (savedVolume >= 0)
				{
					volumeSlider.value = savedVolume;
					bank.bus.setVolume(savedVolume);
				}
				else
				{
					bank.bus.getVolume(out float currentVolume);
					volumeSlider.value = currentVolume;
				}

				volumeSlider.onValueChanged.AddListener((value) => OnVolumeChanged(bank, value));
			}
		}

		private void UpdateSliderValues()
		{
			int index = 0;
			foreach (Transform child in bankControlsContainer)
			{
				if (index >= AudioManager.Instance.Banks.Length) break;

				if (!AudioManager.Instance.Banks[index].bus.isValid())
				{
					index++;
					continue;
				}

				Slider slider = child.Find("VolumeSlider").GetComponent<Slider>();
				AudioManager.Instance.Banks[index].bus.getVolume(out float currentVolume);
				slider.value = currentVolume;

				index++;
			}
		}

		private void OnVolumeChanged(BankVolumeControl bank, float value)
		{
			if (!bank.bus.isValid())
				return;

			bank.bus.setVolume(value);
			_saveData.SetAudioSetting($"Volume_{bank.bankName}", value);
		}

		public void CloseSettings()
		{
			SaveServiceController.Save(_saveData);
			gameObject.SetActive(false);
		}
	}
}