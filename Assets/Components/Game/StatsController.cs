using UnityEngine;

namespace Assets.Components.Game
{
	public class StatsController : MonoBehaviour
	{
		public static int Score;

		private void Update()
		{
			Score += Mathf.FloorToInt(Time.deltaTime);
		}
	}
}
