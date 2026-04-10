using FMOD.Studio;
using FMODUnity;
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

	[Header("Banks Configuration")]
	public BankVolumeControl[] banks = new BankVolumeControl[4]
	{
			new BankVolumeControl { bankName = "Music", busPath = "bus:/Music" },
			new BankVolumeControl { bankName = "SFX", busPath = "bus:/SFX" },
			new BankVolumeControl { bankName = "Ambience", busPath = "bus:/Ambience" },
			new BankVolumeControl { bankName = "UI", busPath = "bus:/UI" }
	};

	private bool controlsCreated = false;

	void Start()
	{
		// Initialize FMOD buses
		foreach (BankVolumeControl bank in banks)
		{
			bank.bus = RuntimeManager.GetBus(bank.busPath);
		}
		closeButton.onClick.AddListener(CloseSettings);
	}

	void OnEnable()
	{
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

	/// <summary>
	/// Initializes the UI controls for each audio bank, setting their initial values based on saved preferences or current bus volumes.
	/// </summary>
	void CreateBankControls()
	{
		foreach (BankVolumeControl bank in banks)
		{
			GameObject control = Instantiate(bankControlPrefab, bankControlsContainer);
			TextMeshProUGUI nameText = control.transform.Find("BankName").GetComponent<TextMeshProUGUI>();
			nameText.text = bank.bankName;
			Slider volumeSlider = control.transform.Find("VolumeSlider").GetComponent<Slider>();
			float savedVolume = PlayerPrefs.GetFloat($"Volume_{bank.bankName}", -1f);
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

	void UpdateSliderValues()
	{
		int index = 0;
		foreach (Transform child in bankControlsContainer)
		{
			if (index >= banks.Length) break;

			Slider slider = child.Find("VolumeSlider").GetComponent<Slider>();
			banks[index].bus.getVolume(out float currentVolume);
			slider.value = currentVolume;

			index++;
		}
	}

	void OnVolumeChanged(BankVolumeControl bank, float value)
	{
		bank.bus.setVolume(value);
		PlayerPrefs.SetFloat($"Volume_{bank.bankName}", value);
	}

	public void CloseSettings()
	{
		PlayerPrefs.Save();
		gameObject.SetActive(false);
	}
}