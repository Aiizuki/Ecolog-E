using Assets.Components.Audio;
using Assets.Components.SaveService;
using Assets.Components.SaveService.Components.SaveService;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the options menu, allowing players to adjust audio settings for different banks (Music, SFX, Ambience, UI) and save their preferences using PlayerPrefs. 
/// </summary>
public class OptionsManager : MonoBehaviour
{
	[System.Serializable]
	public class BankVolumeControl
	{
		public string bankName;
		public string busPath;
		[HideInInspector] public Bus bus;
	}

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
	/// Attends que FMOD soit prêt, initialise les bus, puis crée les contrôles UI.
	/// </summary>
	private IEnumerator InitializeBuses()
	{
		// Attendre que le RuntimeManager FMOD soit pleinement initialisé
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
				Debug.LogWarning($"[OptionsManager] Bus introuvable : '{bank.busPath}'. Vérifiez le chemin dans FMOD Studio.");
			}
		}

		busesInitialized = true;

		// Créer les contrôles maintenant que les bus sont prêts
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
			// Ignorer les bus qui n't ont pas pu être initialisés
			if (!bank.bus.isValid())
			{
				Debug.LogWarning($"[OptionsManager] Bus invalide ignoré : '{bank.busPath}'");
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