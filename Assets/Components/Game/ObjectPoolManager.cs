using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Components.Game
{
    public class ObjectPoolManager : MonoBehaviour
    {
        public static ObjectPoolManager Instance { get; private set; }

        [SerializeField] private PoolSettings poolSettings;

        private readonly Dictionary<string, ObjectPool<GameObject>> _pools = new();
        private readonly Dictionary<GameObject, string> _activeObjects = new();

        #region Unity Lifecycle

        private bool _isQuitting = false;

        private void Awake()
        {
            if (Instance != null && Instance != this) 
            { 
                Destroy(gameObject); 
                return; 
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (GameObject prefab in poolSettings.lstObject)
                RegisterPool(prefab);

            PrewarmPools();
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        #endregion Unity Lifecycle

        #region Public Methods

        /// <summary>
        /// Récupère un objet depuis le pool du prefab donné
        /// </summary>
        public GameObject Get(GameObject prefab)
        {
            if (!_pools.TryGetValue(prefab.name, out ObjectPool<GameObject> pool))
                return null;
            return pool.Get();
        }

        /// <summary>
        /// Retourne un objet à son pool d'origine
        /// </summary>
        public void Release(GameObject obj)
        {
            if (obj == null || _isQuitting)
                return;

            if (!_activeObjects.TryGetValue(obj, out string key))
            {
                Destroy(obj);
                return;
            }

            if (_pools.TryGetValue(key, out ObjectPool<GameObject> pool))
            {
                pool.Release(obj);
                _activeObjects.Remove(obj);
            }
        }

        #endregion Public Methods

        #region Private Helpers

        private void PrewarmPools()
        {
            foreach (GameObject prefab in poolSettings.lstObject)
            {
                if (!_pools.TryGetValue(prefab.name, out ObjectPool<GameObject> pool))
                    continue;

                GameObject[] temp = new GameObject[poolSettings.initialSize];

                for (int i = 0; i < poolSettings.initialSize; i++)
                    temp[i] = pool.Get();

                for (int i = 0; i < poolSettings.initialSize; i++)
                    pool.Release(temp[i]);
            }
        }

        private void RegisterPool(GameObject prefab)
        {
            if (prefab == null || _pools.ContainsKey(prefab.name))
                return;

            string key = prefab.name;

            ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                createFunc: () => CreateObject(prefab),
                actionOnGet: obj => OnGet(obj, key),
                actionOnRelease: obj => OnRelease(obj),
                actionOnDestroy: obj => Destroy(obj),
                collectionCheck: true,
                defaultCapacity: poolSettings.initialSize,
                maxSize: poolSettings.maxSize
            );

            _pools[key] = pool;
        }

        private GameObject CreateObject(GameObject prefab)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.name = prefab.name;
            obj.GetComponent<IPoolable>()?.OnCreatedByPool();
            return obj;
        }

        private void OnGet(GameObject obj, string key)
        {
            _activeObjects[obj] = key;
            obj.transform.SetParent(null);
            obj.SetActive(true);
            obj.GetComponent<IPoolable>()?.OnGetFromPool();
        }

        private void OnRelease(GameObject obj)
        {
            obj.GetComponent<IPoolable>()?.OnReturnToPool();
            obj.SetActive(false);
            obj.transform.SetParent(transform);
        }

        #endregion Private Helpers
    }
}