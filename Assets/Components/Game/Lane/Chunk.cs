using Assets.Components.Game;
using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Components.ObstacleGenerator
{
    public class Chunk : MonoBehaviour, IPoolable
    {
        public ChunkSettings _chunkSettings;

        #region Unity Lifecycle

        private void Update()
        {
            transform.Translate(_chunkSettings.TranslationSpeed * Time.deltaTime * Vector3.back);
        }

        private void OnDestroy()
        {
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<IPoolable>(out _))
                    ObjectPoolManager.Instance.Release(child.gameObject);
            }

            ObjectPoolManager.Instance.Release(gameObject);
            UnityEvents.Instance.GenerateNewChunks.Invoke();
        }

        #endregion Unity Lifecycle

        #region Public Methods

        public bool IsBehindPlayer()
            => _chunkSettings.DeadZoneZIndex >= transform.position.z;

        public void Spawn(Vector3 position)
        {
            transform.position = position;
            // TODO : génération des obstacles dans le chunk
        }

        #endregion Public Methods

        #region IPoolable

        public void OnCreatedByPool() { }

        public void OnGetFromPool()
        {
            transform.position = Vector3.zero;
        }

        public void OnReturnToPool()
        {
            // TODO : clean up obstacles in the chunk
            // TODO : fire an event to spawn a new chunk si nécessaire
        }

        #endregion IPoolable
    }
}