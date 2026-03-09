using Assets.Components.Game;
using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Components.ObstacleGenerator
{
    public class Chunk : MonoBehaviour, IPoolable
    {
        public ChunkSettings _chunkSettings;
        [SerializeField] private bool isDefinedInScene = false;

        #region Unity Lifecycle

        private void Update()
        {
            transform.Translate(_chunkSettings.TranslationSpeed * Time.deltaTime * Vector3.back);
            if(isDefinedInScene && IsBehindPlayer())
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if(isDefinedInScene)
                return;

            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<IPoolable>(out _))
                    ObjectPoolManager.Instance.Release(child.gameObject);
            }

            if(ObjectPoolManager.Instance != null)
                ObjectPoolManager.Instance.Release(gameObject);
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