using Assets.Components.PlayerStats;
using Assets.Components.Singletons;
using UnityEngine;

namespace Assets.Components.UI
{
	public class StatButtonManager : ButtonsManager
	{
		[SerializeField] private EnumUpgradableStat statName;

		public void FireDisplayStatPanel()
		{
			PlayButtonClick();
			UnityEvents.FillStatPanel.Invoke(statName);
		}
	}
}
