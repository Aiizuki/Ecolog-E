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
		[SerializeField] private TextMeshProUGUI oldStats;
		[SerializeField] private TextMeshProUGUI newStats;
		[SerializeField] private TextMeshProUGUI descText;
		[SerializeField] private Image statIcon;
		[SerializeField] private TextMeshProUGUI priceText;
		[SerializeField] private Button updateButton;

		private EnumUpgradableStat statName;

		public void Fill(PlayerStat baseStat, int stat1, int stat2)
		{
			oldStats.text = baseStat.GetStat(stat1).ToString();
			newStats.text = baseStat.GetStat(stat2).ToString();
			descText.text = baseStat.Description;
			statIcon.sprite = baseStat.StatIcon;
			priceText.text = baseStat.GetUpgradeCost(stat2) == -1 ? "Max" : baseStat.GetUpgradeCost(stat2).ToString();
			statName = baseStat.StatName;

			if (!this.gameObject.activeSelf)
				this.gameObject.SetActive(true);

			updateButton.gameObject.SetActive(!priceText.text.Equals("Max"));
		}

		public void UpgradeStat()
		{
			UnityEvents.UpgradeStat.Invoke(statName);
			UnityEvents.FillStatPanel.Invoke(statName);
		}
	}
}