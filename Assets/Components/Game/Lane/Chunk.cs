using Assets.Components.Game.Obstacle;
using Assets.Scripts.Core;
using UnityEngine;

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
            if (isDefinedInScene && IsBehindPlayer())
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (isDefinedInScene)
                return;

            UnityEvents.Instance.ChunkDestroyed.Invoke(this);
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
            Debug.Log($"Obstacle {obstacle.name} given to chunk {name}");
            // TODO : faire un spawn intelligent des obstacles
        }

        #endregion Public Methods
    }
}