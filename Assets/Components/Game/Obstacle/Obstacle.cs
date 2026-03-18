using Assets.Components.ObstacleGenerator;
using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Components.Game.Obstacle
{
    public class Obstacle : MonoBehaviour
    {
        public ObjectPoolManager _pool;

        #region Unity Lifecycle

        private void Start()
        {
            UnityEvents.Instance.ObstacleDestroyed.AddListener(ReturnToPool);
        }

        private void OnDestroy()
        {
            UnityEvents.Instance.ObstacleDestroyed.RemoveListener(ReturnToPool);
        }

        #endregion Unity Lifecycle

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player hit an obstacle!");
                // TODO : add more logic here, such as reducing player health or triggering a game over.
            }
        }

        public void SetPool(ObjectPoolManager obstaclePool)
            => _pool = obstaclePool;

        #region UnityEvents

        private void ReturnToPool(Chunk chunkParent)
        {
            if(transform.parent != chunkParent.transform)
                return;

            if (_pool == null)
            {
                Debug.LogError("Obstacle should return to pool but it is null !");
                return;
            }

            transform.SetParent(_pool.GetPoolParent().transform);
            gameObject.SetActive(false);
        }

        #endregion UnityEvents
    }
}