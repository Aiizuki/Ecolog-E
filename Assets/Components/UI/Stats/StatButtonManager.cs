using Assets.Components.PlayerStats;
using Assets.Components.SaveService;
using Assets.Components.SaveService.Components.SaveService;
using Assets.Components.Singletons;
using Assets.Settings.Player;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Components.UI
{
	public class StatButtonManager : ButtonsManager
	{
		[SerializeField] private EnumUpgradableStat statName;
		[SerializeField] private TextMeshProUGUI statLevelText;
		[SerializeField] private StatsConfig statsConfig;

		private PlayerStat _baseStat;
		private KeyValuePair<EnumUpgradableStat, int> _savedPlayerStat;

		#region Unity Lifecycle

		private void Start()
		{
			InitEvents();
		}

		private new void OnEnable()
		{
			base.OnEnable();
			UpdateStats();

			if (_baseStat == null)
			{
				Debug.LogWarning($"Stat {statName} doesn't exists. Check StatsConfig !");
			}
			else if (_savedPlayerStat.Equals(default(KeyValuePair<PlayerStat, int>)))
			{
				Debug.Log($"Can't get stat {statName} from save file");
				statLevelText.text = $"0/{_baseStat.MaxLevel}";
			}
			else
			{
				statLevelText.text = $"{_savedPlayerStat.Value}/{_baseStat.MaxLevel}";
			}
		}

		private new void OnDisable()
		{
			base.OnDisable();
			RevokeEvents();
		}

		#endregion Unity Lifecycle

		#region Unity Events

		private void InitEvents()
		{
			UnityEvents.UpdateStatText += OnUpdateStatText;
		}

		private void RevokeEvents()
		{
			UnityEvents.UpdateStatText -= OnUpdateStatText;
		}

		#endregion Unity Events

		#region Event Handlers

		private void OnUpdateStatText(EnumUpgradableStat targetStatName, int level)
		{
			if (!targetStatName.Equals(statName))
				return;

			UpdateStats();
			statLevelText.text = $"{level}/{_baseStat.MaxLevel}";
		}

		#endregion Event Handlers

		private void UpdateStats()
		{
			SaveData data = SaveServiceController.Load();

			_savedPlayerStat = data.GetPlayerStatData(statName);
			_baseStat = statsConfig.GetStat(statName);
		}

		public void FireDisplayStatPanel()
		{
			PlayForward();
			UnityEvents.FillStatPanel.Invoke(statName);
		}
	}
}
