using Assets.Components.Game.Chunks.Obstacles;
using UnityEngine;

namespace Assets.Components.Game.Chunks.Lanes
{
	public class Lane : MonoBehaviour
	{
		public void SpawnObstacle(Obstacle obstacle, int position, float distanceBetweenObstacles)
		{
			float startZ = transform.position.z - (GetLaneLength() / 2);
			float zPos = startZ + (position * distanceBetweenObstacles);

			obstacle.transform.SetPositionAndRotation(new Vector3(transform.position.x, 2.5f, zPos), Quaternion.identity);
			obstacle.transform.SetParent(this.transform, true);
			obstacle.gameObject.SetActive(true);
		}

		private float GetLaneLength()
		{
			float meshUnit = GetComponent<Renderer>().localBounds.size.z;
			return transform.localScale.z * meshUnit;
		}
	}
}
