using System.Collections.Generic;
using UnityEngine;

namespace Assets.Components.Game
{
    public class ObjectPoolManager : MonoBehaviour
    {
        [SerializeField] private PoolSettings poolSettings;
        [SerializeField] private GameObject poolParent;

        public readonly Stack<GameObject> Pool = new();
        private readonly HashSet<GameObject> _active = new();

        private bool _isQuitting = false;

        #region Unity Lifecycle

        private void Awake()
        {
            PrewarmPool();
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Récupère une instance depuis le pool.
        /// Si le pool est vide, une nouvelle instance est créée.
        /// </summary>
        public GameObject Get(GameObject prefab)
        {
            GameObject instance = Pool.Count > 0
                ? Pool.Pop()
                : Instantiate(prefab);

            instance.transform.SetParent(poolParent.transform);
            instance.SetActive(true);
            _active.Add(instance);
            return instance;
        }

        /// <summary>
        /// Retourne une instance au pool.
        /// </summary>
        public void Release(GameObject instance)
        {
            if (instance == null || _isQuitting)
                return;

            if (!_active.Contains(instance))
            {
                Debug.LogWarning($"[ObjectPoolManager] Instance inconnue du pool : {instance.name}");
                Destroy(instance);
                return;
            }

            instance.SetActive(false);
            _active.Remove(instance);
            Pool.Push(instance);
        }

        #endregion

        #region Private Helpers

        private void PrewarmPool()
        {
            for (int i = 0; i < poolSettings.initialSize; i++)
            {
                int index = i % poolSettings.lstObject.Count; // Permet de répartir les instances entre les différents types d'objets
                GameObject instance = Instantiate(poolSettings.lstObject[index]);

                instance.transform.SetParent(poolParent.transform);
                instance.SetActive(false);
                Pool.Push(instance);
            }
        }

        #endregion
    }
}