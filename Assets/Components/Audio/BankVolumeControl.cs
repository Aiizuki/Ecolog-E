using FMOD.Studio;
using UnityEngine;

namespace Assets.Components.Audio
{
	[System.Serializable]
	public class BankVolumeControl
	{
		public string bankName;
		public string busPath;
		[HideInInspector] public Bus bus;
	}
}