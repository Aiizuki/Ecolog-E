using Assets.Components.PlayerStats;
using System.Collections.Generic;

namespace Assets.Components.SaveService
{
	[System.Serializable]
	public class SaveData
	{
		public int RunCount;
		public int Score;
		public Dictionary<EnumUpgradableStat, int> LstPlayerStats;

		public SaveData()
		{
			RunCount = 0;
			Score = 0;
			LstPlayerStats = new();
		}
	}
}
