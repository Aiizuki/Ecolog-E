using Assets.Components.PlayerStats;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Settings.Player
{
	[CreateAssetMenu(fileName = "StatsConfig", menuName = "Scriptable Objects/StatsConfig")]
	public class StatsConfig : ScriptableObject
	{
		public List<PlayerStat> LstAvailableStats;

		public float? GetStatValue(EnumUpgradableStat statName, int playerLevel)
		{
			PlayerStat stat = LstAvailableStats.Find(s => s.StatName.Equals(statName));
			float? value = stat?.GetStat(playerLevel);

			if (value == null)
				Debug.LogWarning($"Can't get value for {statName} at level {playerLevel}");

			return value;
		}
	}
}