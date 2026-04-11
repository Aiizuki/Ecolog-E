using Assets.Components.SaveService;
using Assets.Components.SaveService.Components.SaveService;
using Assets.Components.Singletons;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OptionsManager;

namespace Assets.Components.Audio
{
	public class AudioManager : MonoBehaviour
	{
		[Header("Banks Configuration")]
		public BankVolumeControl[] Banks = new BankVolumeControl[3]
		{
			new() { bankName = "Music",    busPath = "bus:/Music"    },
			new() { bankName = "SFX",      busPath = "bus:/SFX"      },
			new() { bankName = "UI",       busPath = "bus:/UI"       }
		};

		public static AudioManager Instance { get; private set; }
		private List<EventInstance> _lstEventInstances;
		private List<StudioEventEmitter> _lstEventEmitters;

		public EventInstance MusicEventInstance;
		public EventInstance AmbienceInstance;
		public EventInstance BonusZoneInstance;

		private SaveData _saveData;

		#region Unity Lifecycle

		private void Awake()
		{
			if (Instance != null)
				return;
			Instance = this;

			_lstEventInstances = new List<EventInstance>();
			_lstEventEmitters = new List<StudioEventEmitter>();
			_saveData = SaveServiceController.Load();

			DontDestroyOnLoad(gameObject);
		}

		private void Start()
		{
			InitEvents();
			StartCoroutine(InitializeAudioWhenReady());
		}

		private void OnApplicationQuit()
		{
			CleanSounds();
		}

		#endregion Unity Lifecycle

		#region Unity Events

		private void InitEvents()
		{
			UnityEvents.Instance.GameOver.AddListener(GameOverAmbiance);
			UnityEvents.ReturnToHome += HomeMenuAmbiance;
			UnityEvents.Instance.CriticalHealthStart.AddListener(CriticalAmbianceStart);
			UnityEvents.Instance.CriticalHealthEnd.AddListener(CriticalAmbianceEnd);
		}

		#endregion Unity Events

		private IEnumerator InitializeAudioWhenReady()
		{
			while (!RuntimeManager.HasBankLoaded("Master"))
			{
				yield return null;
			}

			foreach (BankVolumeControl bank in Banks)
			{
				try
				{
					bank.bus = RuntimeManager.GetBus(bank.busPath);
				}
				catch (BusNotFoundException)
				{
					Debug.LogWarning($"[AudioManager] Bus introuvable : '{bank.busPath}'");
					continue;
				}

				float volume = _saveData.LstSoundSettings.ContainsKey($"Volume_{bank.bankName}") ? _saveData.LstSoundSettings[$"Volume_{bank.bankName}"] : 0.5f;
				bank.bus.setVolume(volume);
				HomeMenuAmbiance();
			}
		}

		/// <summary>
		/// Play a one-shot sound at a specific position in the world
		/// </summary>
		public void PlayOneShot(EventReference sound, Vector2 pos)
			=> RuntimeManager.PlayOneShot(sound, pos);

		/// <summary>
		/// Creates a new event instance from the specified event reference.
		/// </summary>
		/// <param name="reference">The event reference used to create the event instance.</param>
		/// <param name="intouchable">If true, the event instance is not tracked (so it can't be stopped by <see cref="CleanSounds()"/>)</param>
		/// <returns>A new EventInstance created from the specified reference.</returns>
		public EventInstance CreateEventInstance(EventReference reference, bool intouchable = false)
		{
			EventInstance eventInstance = RuntimeManager.CreateInstance(reference);

			if (!intouchable)
				_lstEventInstances.Add(eventInstance);

			return eventInstance;
		}

		/// <summary>
		/// Setups the ambiance for the HomeMenu screen
		/// </summary>
		private void HomeMenuAmbiance()
		{
			CleanSounds();
			MusicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			MusicEventInstance = CreateEventInstance(FMODEvents.Instance.MainMusic, intouchable: true);

			// V�rifier que l'instance est valide
			if (MusicEventInstance.isValid())
			{
				FMOD.RESULT result = MusicEventInstance.start();
				Debug.Log($"[FMOD] Music started with result: {result}");
			}
			else
			{
				Debug.LogError("[FMOD] MusicEventInstance is not valid!");
			}
		}

		/// <summary>
		/// Setups the ambiance for the GameOver screen
		/// </summary>
		private void GameOverAmbiance()
		{
			CleanSounds();

		}

		/// <summary>
		/// Setups the ambiance for the GameOver screen
		/// </summary>
		public void GameAmbiance()
		{
			CleanSounds();
			MusicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

			//RuntimeManager.StudioSystem.setParameterByName(FMODEvents.GameOverEvent, 0f);

			MusicEventInstance = CreateEventInstance(FMODEvents.Instance.GameMusic, intouchable: true);

			// V�rifier que l'instance est valide
			if (MusicEventInstance.isValid())
			{
				FMOD.RESULT result = MusicEventInstance.start();
				Debug.Log($"[FMOD] Music started with result: {result}");
			}
			else
			{
				Debug.LogError("[FMOD] MusicEventInstance is not valid!");
			}
		}

		private void CriticalAmbianceStart()
			=> StartCoroutine(TransitionToCritical(true));

		private void CriticalAmbianceEnd()
			=> StartCoroutine(TransitionToCritical(false));

		private IEnumerator TransitionToCritical(bool start)
		{
			while (true)
			{
				RuntimeManager.StudioSystem.getParameterByName("CriticalHealth", out float value);

				if (start)
				{
					if (value >= 1f)
						yield break;
					RuntimeManager.StudioSystem.setParameterByName("CriticalHealth", Mathf.Min(value + 0.1f, 1f));
				}
				else
				{
					if (value <= 0f)
						yield break;
					RuntimeManager.StudioSystem.setParameterByName("CriticalHealth", Mathf.Max(value - 0.1f, 0f));
				}

				yield return new WaitForFixedUpdate();
			}
		}

		/// <summary>
		/// Clean all sounds, stop and release all EventInstances, stop all StudioEventEmitters.
		/// </summary>
		public void CleanSounds()
		{
			if (_lstEventInstances != null)
			{
				foreach (EventInstance sound in _lstEventInstances)
				{
					sound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
					sound.release();
				}
			}
			if (_lstEventEmitters != null)
			{
				foreach (StudioEventEmitter sound in _lstEventEmitters)
				{
					sound.Stop();
				}
			}
		}

		/// <summary>
		/// Switches a FMOD parameter by name, if the parameter is 0, it will be set to 1, and if it's 1, it will be set to 0.
		/// </summary>
		public static void SwitchBooleanParameter(string @event)
		{
			RuntimeManager.StudioSystem.getParameterByName(@event, out float paramValue);
			FMOD.RESULT result = RuntimeManager.StudioSystem.setParameterByName(@event, paramValue == 0f ? 1f : 0f);
			Debug.Log($"[FMOD] Changement param global {@event} : {result}");
		}
	}
}