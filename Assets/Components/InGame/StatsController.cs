using Assets.Components.PlayerStats;
using Assets.Components.SaveService;
using Assets.Components.SaveService.Components.SaveService;
using Assets.Components.Singletons;
using Assets.Settings.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Components.Game
{
	public class StatsController : MonoBehaviour
	{
		public static int Score;
		public static int InGameTime;
		public static int TrashCollected;
		public static int MoneyEarned;

		private SaveData _saveData;
		[SerializeField] private StatsConfig _statsConfig;

		private void Awake()
		{
			_saveData = SaveServiceController.Load();
		}

		private void Start()
		{
			InitEvents();
		}

		private void OnDestroy()
		{
			RevokeEvents();
		}

		#region Unity Events

		private void InitEvents()
		{
			UnityEvents.Instance.NewGame.AddListener(OnNewGame);
			UnityEvents.Instance.GameOver.AddListener(OnGameOver);
			UnityEvents.OnTrashCollected += OnTrashCollected;
			UnityEvents.OnComponentCollected += OnComponentCollected;
		}

		private void RevokeEvents()
		{
			UnityEvents.Instance.NewGame.RemoveListener(OnNewGame);
			UnityEvents.Instance.GameOver.RemoveListener(OnGameOver);
			UnityEvents.OnTrashCollected -= OnTrashCollected;
			UnityEvents.OnComponentCollected -= OnComponentCollected;
		}

		private static void OnTrashCollected()
			=> TrashCollected++;

		private static void OnComponentCollected()
			=> MoneyEarned++;

		#endregion UnityEvents

		#region Static Helpers

		public static void AddScore(int score)
		{
			Score += score;
			UnityEvents.ScoreUpdate.Invoke(Score);
		}

		public float GetPlayerHealth(int basePlayerHealth)
		{
			KeyValuePair<EnumUpgradableStat, int> stat = _saveData.LstPlayerStats.FirstOrDefault(x => x.Key == EnumUpgradableStat.Reinforcement);
			if (!stat.Equals(default(KeyValuePair<PlayerStat, int>)))
			{
				return basePlayerHealth + _statsConfig.GetStatValue(EnumUpgradableStat.Reinforcement, stat.Value) ?? basePlayerHealth;
			}
			return basePlayerHealth;
		}

		public float GetPlayerDamageReduction()
		{
			KeyValuePair<EnumUpgradableStat, int> stat = _saveData.LstPlayerStats.FirstOrDefault(x => x.Key == EnumUpgradableStat.Shielding);
			if (!stat.Equals(default(KeyValuePair<PlayerStat, int>)))
			{
				return _statsConfig.GetStatValue(EnumUpgradableStat.Shielding, stat.Value) ?? 0;
			}
			return 0;
		}

		public float GetPlayerInvicibilityDuration(float baseDuration)
		{
			KeyValuePair<EnumUpgradableStat, int> stat = _saveData.LstPlayerStats.FirstOrDefault(x => x.Key == EnumUpgradableStat.Stabilizer);
			if (!stat.Equals(default(KeyValuePair<PlayerStat, int>)))
			{
				return baseDuration + _statsConfig.GetStatValue(EnumUpgradableStat.Stabilizer, stat.Value) ?? baseDuration;
			}
			return baseDuration;
		}

		public float GetPlayerHealthLooseRatio(float basePlayerHealthLooseRatio)
		{
			KeyValuePair<EnumUpgradableStat, int> stat = _saveData.LstPlayerStats.FirstOrDefault(x => x.Key == EnumUpgradableStat.Optimization);
			if (!stat.Equals(default(KeyValuePair<PlayerStat, int>)))
			{
				return basePlayerHealthLooseRatio + _statsConfig.GetStatValue(EnumUpgradableStat.Optimization, stat.Value) ?? basePlayerHealthLooseRatio;
			}
			return basePlayerHealthLooseRatio;
		}

		#endregion Static Helpers

		#region Private Methods

		private void OnNewGame()
		{
			Score = 0;
			TrashCollected = 0;
			MoneyEarned = 0;
			InGameTime = 0;
		}

		private void OnGameOver()
		{
			_saveData.Score = Score;
			_saveData.RunCount++;
			_saveData.TrashCollected = TrashCollected;
			_saveData.MoneyEarned = MoneyEarned;
			_saveData.LastRunTime = InGameTime;

			SaveServiceController.Save(_saveData);
		}

		#endregion Private Methods
	}
}
