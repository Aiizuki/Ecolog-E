using Assets.Components.PlayerStats;
using Assets.Components.Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Components.UI.Stats
{
	public class StatPanelManager : MonoBehaviour
	{
		[Header("Stats Core")]
		[SerializeField] private Image statIcon;
		[SerializeField] private TextMeshProUGUI oldStats;
		[SerializeField] private TextMeshProUGUI newStats;
		[SerializeField] private TextMeshProUGUI statNameText;
		[SerializeField] private TextMeshProUGUI descText;

		[Header("Stat costs")]
		[SerializeField] private GameObject statPriceGO;
		[SerializeField] private TextMeshProUGUI priceText;
		[SerializeField] private Button upgradeButton;
		[SerializeField] private TextMeshProUGUI upgradeButtonText;

		private EnumUpgradableStat statName;

		public void Fill(PlayerStat baseStat, int stat1, int stat2)
		{
			oldStats.text = baseStat.GetStat(stat1).ToString();
			newStats.text = baseStat.GetStat(stat2).ToString();
			statNameText.text = baseStat.StatName.ToString();
			descText.text = baseStat.Description;
			statIcon.sprite = baseStat.StatIcon;

			priceText.text = baseStat.GetUpgradeCost(stat2) == -1 ? "Max" : baseStat.GetUpgradeCost(stat2).ToString();
			upgradeButtonText.text = baseStat.GetUpgradeCost(stat2) == -1 ? "Max" : "Upgrade";
			statPriceGO.SetActive(!priceText.text.Equals("Max"));
			upgradeButton.interactable = !priceText.text.Equals("Max");

			statName = baseStat.StatName;

			if (!this.gameObject.activeSelf)
				this.gameObject.SetActive(true);
		}

		public void UpgradeStat()
		{
			UnityEvents.UpgradeStat.Invoke(statName);
			UnityEvents.FillStatPanel.Invoke(statName);
		}
	}
}