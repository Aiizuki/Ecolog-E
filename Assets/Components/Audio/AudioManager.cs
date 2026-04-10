using Assets.Components.Singletons;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Components.Audio
{
	public class AudioManager : MonoBehaviour
	{
		public static AudioManager Instance { get; private set; }
		private List<EventInstance> _lstEventInstances;
		private List<StudioEventEmitter> _lstEventEmitters;

		public EventInstance MusicEventInstance;
		public EventInstance AmbienceInstance;
		public EventInstance BonusZoneInstance;

		private void Awake()
		{
			if (Instance != null)
				return;
			Instance = this;

			_lstEventInstances = new List<EventInstance>();
			_lstEventEmitters = new List<StudioEventEmitter>();

			UnityEvents.Instance.GameOver.AddListener(() => GameOverAmbiance());
			UnityEvents.ReturnToHome += HomeMenuAmbiance;

			DontDestroyOnLoad(gameObject);
		}

		private void Start()
		{
			StartCoroutine(InitializeAudioWhenReady());
		}

		private IEnumerator InitializeAudioWhenReady()
		{
			// Wait until FMOD is initialized and the Master bank is loaded
			while (!RuntimeManager.HasBankLoaded("Master"))
			{
				yield return null;
			}

			//InitializeAmbiance(FMODEvents.Instance.CaveAmbiance, FMODEvents.Instance.MainMusic);
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
		/// Initializes the ambiance by starting the music and ambience event instances, and setting the GameOver parameter to 0 if it's not already. 
		/// This method is called when entering the HomeMenu screen to ensure the correct ambiance is set
		/// </summary>
		private void InitializeAmbiance(EventReference ambience, EventReference music)
		{
			RuntimeManager.StudioSystem.setParameterByName(FMODEvents.GameOverEvent, 0f);

			MusicEventInstance = CreateEventInstance(music, intouchable: true);
			AmbienceInstance = CreateEventInstance(ambience);

			// Vérifier que l'instance est valide
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
		/// Setups the ambiance for the HomeMenu screen
		/// </summary>
		private void HomeMenuAmbiance()
		{
			CleanSounds();
			MusicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			//if (RuntimeManager.StudioSystem.getParameterByName(FMODEvents.GameOverEvent, out _) != 0)
			//	SwitchBooleanParameter(FMODEvents.GameOverEvent);
			//InitializeAmbiance(FMODEvents.Instance.CaveAmbiance, FMODEvents.Instance.MainMusic);
		}

		/// <summary>
		/// Setups the ambiance for the GameOver screen
		/// </summary>
		private void GameOverAmbiance()
		{
			CleanSounds();
			//RuntimeManager.StudioSystem.getParameterByName(FMODEvents.GameOverEvent, out float paramValue);
			//if (paramValue != null && paramValue != 1)
			//	SwitchBooleanParameter(FMODEvents.GameOverEvent);
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

		private void OnApplicationQuit()
		{
			CleanSounds();
		}
	}
}