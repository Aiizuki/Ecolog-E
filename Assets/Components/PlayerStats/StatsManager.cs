using Assets.Components.SaveService;
using Assets.Components.SaveService.Components.SaveService;
using Assets.Components.Singletons;
using Assets.Components.UI.Stats;
using Assets.Settings.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Components.PlayerStats
{
	public class StatsManager : MonoBehaviour
	{
		private SaveData _saveData;
		private Dictionary<PlayerStat, int> _playerStats;

		[SerializeField] private StatsConfig _availableStats;
		[SerializeField] private StatPanelManager statPanel;

		[Header("UI References")]
		public Button closeButton;

		#region Unity Lifecycle

		private void Start()
		{
			_saveData = SaveServiceController.Load();

			_playerStats = _saveData.LstPlayerStats
				.Select(entry => new
				{
					Stat = _availableStats.LstAvailableStats.FirstOrDefault(s => s.StatName == entry.Key),
					entry.Value
				})
				.Where(x => x.Stat != null)
				.ToDictionary(x => x.Stat, x => x.Value);

			InitEvents();
		}

		private void OnDestroy()
		{
			RevokeEvents();
		}

		#endregion Unity Lifecycle

		#region Unity Events

		private void InitEvents()
		{
			UnityEvents.UpgradeStat += OnUpgradeStat;
			UnityEvents.FillStatPanel += OnFillStatPanel;

			closeButton.onClick.AddListener(ClosePanel);
		}

		private void RevokeEvents()
		{
			UnityEvents.UpgradeStat -= OnUpgradeStat;
			UnityEvents.ReturnToHome -= OnReturnToHome;

			closeButton.onClick.RemoveListener(ClosePanel);
		}

		private void OnFillStatPanel(EnumUpgradableStat statName)
		{
			if (!IsStatValid(statName, out PlayerStat baseStat))
				return;

			KeyValuePair<PlayerStat, int> stat = _playerStats.FirstOrDefault(x => x.Key.StatName == statName);
			if (stat.Equals(default(KeyValuePair<PlayerStat, int>)))
			{
				statPanel.Fill(baseStat, 0, 1);
				return;
			}
			statPanel.Fill(baseStat, stat.Value, stat.Value + 1);
		}

		/// <summary>
		/// This event is called every time the player click on an upgrade/unlock button
		/// </summary>
		/// <param name="statName">The name of the stat linked to the button clicked</param>
		private void OnUpgradeStat(EnumUpgradableStat statName)
		{
			if (!IsStatValid(statName, out PlayerStat baseStat))
				return;

			KeyValuePair<PlayerStat, int> stat = _playerStats.FirstOrDefault(x => x.Key.StatName == statName);
			if (!stat.Equals(default(KeyValuePair<PlayerStat, int>)))
			{
				_playerStats[stat.Key]++;
			}
			else
			{
				_playerStats.Add(baseStat, 1);
			}
		}

		private bool IsStatValid(EnumUpgradableStat statName, out PlayerStat baseStat)
		{
			baseStat = _availableStats.LstAvailableStats.Find(s => s.StatName.Equals(statName));
			if (baseStat == null)
			{
				Debug.LogWarning($"{statName} is not a valid stat !");
				return false;
			}
			return true;
		}

		private void OnReturnToHome()
		{
			SaveStats();
		}

		#endregion Unity Events

		public void ClosePanel()
		{
			SaveStats();
			gameObject.SetActive(false);
		}

		private void SaveStats()
		{
			_saveData.LstPlayerStats = _playerStats
				.Where(x => x.Key != null)
				.GroupBy(x => x.Key.StatName)
				.ToDictionary(
					g => g.Key,
					g => g.First().Value
				);

			SaveServiceController.Save(_saveData);
		}
	}
}