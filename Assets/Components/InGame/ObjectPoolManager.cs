using Assets.Scripts.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Components.Game
{
	public class ObjectPoolManager : MonoBehaviour
	{
		[SerializeField] private PoolSettings poolSettings;
		[SerializeField] private GameObject poolParent;

		public Stack<GameObject> Pool = new();
		private readonly HashSet<GameObject> _active = new();

		private bool _isQuitting = false;
		public int _poolSize = 0;

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
		public GameObject Get(bool shuffle = false)
		{
			if (Pool.Count == 0 && _poolSize <= poolSettings.maxSize)
			{
				AddRandomObjectToPool();
				_poolSize++;
			}
			else if (Pool.Count == 0 && _poolSize >= poolSettings.maxSize)
				throw new UnityException("Max pool size reached but still trying to instantiate new prefabs");

			if (shuffle)
				Pool = RandomisationHelper.ShuffleStack(Pool);

			GameObject instance = Pool.Pop();
			instance.transform.SetParent(poolParent.transform, true);
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

			instance.transform.SetParent(poolParent.transform, true);
			instance.SetActive(false);
			_active.Remove(instance);
			Pool.Push(instance);
		}

		#endregion Public Methods

		#region Private Helpers

		private void PrewarmPool()
		{
			for (int i = 0; i < poolSettings.initialSize; i++)
			{
				int index = i % poolSettings.lstObject.Count; // Permet de répartir les instances entre les différents types d'objets
				GameObject instance = Instantiate(poolSettings.lstObject[index], poolParent.transform);
				instance.SetActive(false);
				Pool.Push(instance);
			}

			_poolSize = poolSettings.initialSize;
		}

		private void AddRandomObjectToPool()
		{
			GameObject randomObject = RandomisationHelper.GetRandomItemFromList(poolSettings.lstObject);
			GameObject instance = Instantiate(randomObject, poolParent.transform);
			instance.SetActive(false);
			Pool.Push(instance);
		}

		#endregion
	}
}