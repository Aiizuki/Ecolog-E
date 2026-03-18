using Assets.Components.Game.Obstacle;
using Assets.Scripts.Core;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Assets.Components.ObstacleGenerator
{
    public class Chunk : MonoBehaviour
    {
        public ChunkSettings _chunkSettings;
        [SerializeField] private bool isDefinedInScene = false;

        #region Unity Lifecycle

        private void Update()
        {
            transform.Translate(_chunkSettings.TranslationSpeed * Time.deltaTime * Vector3.back);
            if (IsBehindPlayer())
            {
                if(isDefinedInScene)
                    Destroy(gameObject);

                UnityEvents.Instance.ChunkDestroyed.Invoke(this);
                UnityEvents.Instance.ObstacleDestroyed.Invoke(this);
            }
        }

        #endregion Unity Lifecycle

        #region Public Methods

        public bool IsBehindPlayer()
            => _chunkSettings.DeadZoneZIndex >= transform.position.z;

        public void Spawn(Vector3 position)
        {
            transform.position = position;
            UnityEvents.Instance.GenerateNewObstacles.Invoke(this);
        }

        public void GiveObstacle(Obstacle obstacle)
        {
            obstacle.transform.SetPositionAndRotation(this.transform.position, Quaternion.identity);
            obstacle.transform.SetParent(this.transform);
            obstacle.gameObject.SetActive(true);
        }

        #endregion Public Methods
    }
}