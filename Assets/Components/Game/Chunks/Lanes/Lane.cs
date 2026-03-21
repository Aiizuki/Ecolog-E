using Assets.Components.Game.Chunks.Obstacles;
using UnityEngine;

namespace Assets.Components.Game.Chunks.Lanes
{
	public class Lane : MonoBehaviour
	{
		private int _nbObstacleInLane = 0;

		public void SpawnObstacle(Obstacle obstacle, int position)
		{
			position = position % (int)transform.localScale.z;

			if (position == 0)
				return; // We avoid spawning at the very edge of the lane

			float meshUnit = GetComponent<Renderer>().localBounds.size.z;
			float zPos = (transform.position.z - ((transform.localScale.z / 2) * meshUnit)) + (position * meshUnit);

			obstacle.transform.SetPositionAndRotation(new Vector3(transform.position.x, 2.5f, zPos), Quaternion.identity);
			obstacle.transform.SetParent(this.transform);
			obstacle.gameObject.SetActive(true);
			_nbObstacleInLane++;
		}

		public bool IsFull()
			=> _nbObstacleInLane >= Mathf.FloorToInt(transform.localScale.z);
	}
}
