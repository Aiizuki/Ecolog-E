using Assets.Components.Singletons;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Assets.Components.Cameras
{
	/// <summary>
	/// When the player enters/exits the critical health situation, we add/remove VFX for feedback
	/// </summary>
	public class CriticalHealthFX : MonoBehaviour
	{
		[SerializeField] private Volume _volume;

		private Vignette _vignette;

		#region Unity Lifecycle

		private void Start()
		{
			_volume.profile.TryGet(out _vignette);
			InitEvents();
		}

		private void OnDestroy()
		{
			RevokeEvents();
		}

		#endregion Unity Lifecycle

		#region UnityEvents

		private void InitEvents()
		{
			UnityEvents.Instance.CriticalHealthStart.AddListener(OnCriticalHealthStart);
			UnityEvents.Instance.CriticalHealthEnd.AddListener(OnCriticalHealthEnd);
		}

		private void RevokeEvents()
		{
			UnityEvents.Instance.CriticalHealthStart.RemoveListener(OnCriticalHealthStart);
			UnityEvents.Instance.CriticalHealthEnd.RemoveListener(OnCriticalHealthEnd);
		}

		#endregion UnityEvents

		private void OnCriticalHealthStart()
			=> _vignette.active = true;

		private void OnCriticalHealthEnd()
			=> _vignette.active = false;
	}
}