using System.Collections.Generic;
using UnityEngine;

namespace Assets.Components.PlayerStats
{
	[CreateAssetMenu(fileName = "PlayerStat", menuName = "Scriptable Objects/PlayerStat")]
	public class PlayerStat : ScriptableObject
	{
		public EnumUpgradableStat StatName;

		[SerializeField] private List<int> LstCostPerLevel;
		[SerializeField] private List<float> LstCoefPerLevel;

		public string Description;
		public Sprite StatIcon;

		public int MaxLevel => LstCostPerLevel.Count;

		public float? GetStat(int playerLevel)
		{
			if (playerLevel == 0)
			{
				Debug.Log("Current level is 0 so no stat");
				return 0;
			}

			if (LstCoefPerLevel?.Count < playerLevel)
			{
				Debug.LogWarning($"{StatName} doesn't have {playerLevel} levels ! (max : {LstCoefPerLevel.Count})");
				return null;
			}

			return LstCoefPerLevel[playerLevel - 1];
		}

		public float GetUpgradeCost(int playerLevel)
		{
			if (LstCostPerLevel.Count < playerLevel)
			{
				Debug.LogWarning($"{StatName} doesn't have a cost at level {playerLevel}");
				return -1;
			}

			return LstCostPerLevel[playerLevel - 1];
		}
	}
}
