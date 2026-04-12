using Assets.Components.Audio;
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
		[SerializeField] private PlayerMoneyController _playerMoneyController;
		[SerializeField] private ToastNotification _toastNotification;

		[Header("UI References")]
		public Button closeButton;

		#region Unity Lifecycle

		private void Start()
		{
			_saveData = SaveServiceController.Load();
			_playerMoneyController.Init(_saveData.PlayerComponentTotal);

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
			UnityEvents.FillStatPanel -= OnFillStatPanel;

			closeButton.onClick.RemoveListener(ClosePanel);
		}

		#endregion Unity Events

		#region Event Handlers

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
			bool isNewStat = stat.Equals(default(KeyValuePair<PlayerStat, int>));

			PlayerStat key = isNewStat ? baseStat : stat.Key;
			int currentLevel = isNewStat ? 0 : stat.Value;
			int statCost = key.GetUpgradeCost(isNewStat ? 1 : currentLevel);

			if (!PlayerMoneyController.HasMoney(_saveData.PlayerComponentTotal, statCost))
			{
				AudioManager.PlayInstanceOneTime(FMODEvents.Instance.NotificationFailure);
				ToastNotification.Show("You don't have enought money", 3f);
				return;
			}

			AudioManager.PlayInstanceOneTime(FMODEvents.Instance.NotificationSuccess);
			_saveData.PlayerComponentTotal = _playerMoneyController.Pay(_saveData.PlayerComponentTotal, statCost);

			if (isNewStat)
			{
				_playerStats.Add(baseStat, 1);
				UnityEvents.UpdateStatText.Invoke(statName, 1);
			}
			else
			{
				_playerStats[stat.Key]++;
				UnityEvents.UpdateStatText.Invoke(statName, _playerStats[stat.Key]);
			}

			UnityEvents.FillStatPanel.Invoke(statName);
		}

		#endregion Event Handlers

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