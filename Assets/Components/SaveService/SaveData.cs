using Assets.Components.PlayerStats;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Components.SaveService
{
	[System.Serializable]
	public class SaveData
	{
		public Dictionary<EnumUpgradableStat, int> LstPlayerStats;
		public int PlayerComponentTotal;

		public int PlayerBestScore;
		public int PlayerBestRunTime;
		public int PlayerBestTrashCollected;

		#region InGame stats

		public int RunCount;
		public int RunScore;
		public float LastRunTime;
		public int TrashCollected;
		public int ComponentsCollected;

		#endregion InGame stats

		public SaveData()
		{
			RunCount = 0;
			RunScore = 0;
			LastRunTime = 0;
			TrashCollected = 0;
			ComponentsCollected = 0;

			LstPlayerStats = new();
		}

		public KeyValuePair<EnumUpgradableStat, int> GetPlayerStatData(EnumUpgradableStat statName)
			=> LstPlayerStats.FirstOrDefault(kvp => kvp.Key == statName);
	}
}
