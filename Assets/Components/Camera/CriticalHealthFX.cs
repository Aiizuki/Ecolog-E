using Assets.Components.Singletons;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Assets.Components.Camera
{
	public class CriticalHealthFX : MonoBehaviour
	{
		[SerializeField] private Volume _volume;

		private Vignette _vignette;

		private void Start()
		{
			_volume.profile.TryGet(out _vignette);

			UnityEvents.Instance.CriticalHealthEvent.AddListener(OnCriticalHealthStart);
			UnityEvents.Instance.EndCriticalHealthEvent.AddListener(OnCriticalHealthEnd);
		}

		private void OnCriticalHealthStart()
			=> _vignette.active = true;

		private void OnCriticalHealthEnd()
			=> _vignette.active = false;
	}
}