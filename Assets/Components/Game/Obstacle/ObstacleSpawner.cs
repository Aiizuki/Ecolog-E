using Assets.Components.ObstacleGenerator;
using Assets.Scripts.Core;
using Assets.Scripts.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Components.Game.Obstacle
{
    public class ObstacleSpawner : MonoBehaviour
    {
        [SerializeField] private ObjectPoolManager _obstaclePool;

        #region Unity Lifecycle

        private void Start()
        {
            UnityEvents.Instance.GenerateNewObstacles.AddListener(SpawnObstacles);
        }

        private void OnDestroy()
        {
            UnityEvents.Instance.GenerateNewObstacles.RemoveListener(SpawnObstacles);
        }

        #endregion Unity Lifecycle

        private void SpawnObstacles(Chunk chunk)
        {
            if (_obstaclePool.Pool.Count == 0)
            {
                Debug.LogError("No obstacle in pool");
                return;
            }

            if (chunk.GetNbLanes() == 0)
            {
                Debug.LogWarning("The given chunk has no lanes defined !");
                return;
            }

            List<Obstacle> lstObstacle = new();
            for (int i = 1; i < chunk.GetNbLanes(); i++)
            {
                GameObject prefab = RandomisationHelper.GetRandomItemFromStack(_obstaclePool.Pool);
                GameObject obj = _obstaclePool.Get(prefab);

                if (obj == null)
                    continue;

                Obstacle obstacle = obj.GetComponent<Obstacle>();

                obstacle.SetPool(this._obstaclePool);
                lstObstacle.Add(obstacle);
            }

            chunk.GiveObstacle(lstObstacle);
        }
    }
}