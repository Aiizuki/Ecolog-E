using Assets.Components.PlayerStats;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Components.SaveService
{
	[System.Serializable]
	public class SaveData
	{
		public Dictionary<EnumUpgradableStat, int> LstPlayerStats;

		#region InGame stats

		public int RunCount;
		public int Score;
		public float LastRunTime;
		public int TrashCollected;
		public int MoneyEarned;

		#endregion InGame stats

		public SaveData()
		{
			RunCount = 0;
			Score = 0;
			LastRunTime = 0;
			TrashCollected = 0;
			MoneyEarned = 0;

			LstPlayerStats = new();
		}

		public KeyValuePair<EnumUpgradableStat, int> GetPlayerStatData(EnumUpgradableStat statName)
			=> LstPlayerStats.FirstOrDefault(kvp => kvp.Key == statName);
	}
}
