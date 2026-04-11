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

		#region Audio settings

		public Dictionary<string, float> LstSoundSettings;

		#endregion Audio settings

		public SaveData()
		{
			RunCount = 0;
			RunScore = 0;
			LastRunTime = 0;
			TrashCollected = 0;
			ComponentsCollected = 0;

			LstPlayerStats = new();
			LstSoundSettings = new();
		}

		public KeyValuePair<EnumUpgradableStat, int> GetPlayerStatData(EnumUpgradableStat statName)
			=> LstPlayerStats.FirstOrDefault(kvp => kvp.Key == statName);

		public void SetAudioSetting(string busName, float value)
		{
			if (LstSoundSettings.ContainsKey(busName))
				LstSoundSettings[busName] = value;
			else
				LstSoundSettings.Add(busName, value);
		}
	}
}
