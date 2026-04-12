using TMPro;
using UnityEngine;

namespace Assets.Components.PlayerStats
{
	public class PlayerMoneyController : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _moneyText;

		public void Init(int playerMoney)
		{
			_moneyText.text = playerMoney.ToString();
		}

		public static bool HasMoney(int playerMoney, int statUpgradeCost)
			=> playerMoney >= statUpgradeCost;

		public int Pay(int playerMoney, int statUpgradeCost)
		{
			int newPlayerMoney = playerMoney - statUpgradeCost;
			_moneyText.text = newPlayerMoney.ToString();
			return newPlayerMoney;
		}
	}
}