using Assets.Components.Game;
using Assets.Components.Game.Chunks;
using Assets.Components.InGame.Chunks.Interactables;
using Assets.Components.Singletons;
using Assets.Scripts.Helpers;
using Assets.Settings.GameDefilement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Components.InGame.Ennemy
{
	public class EnnemyController : AInteractable
	{
		[SerializeField] private List<ProjectileController> _lstProjectile;
		[SerializeField] private EnnemySettings _ennemySettings;
		[SerializeField] private GameObject _ennemyPivot;
		[SerializeField] private Animator _animator;
		[SerializeField] private Transform _projectileSpawnPoint;

		private GameObject _player;
		private Transform _playerTransform;
		private Coroutine _patternRoutine;

		private bool _isPlaying = true;
		private Chunk _chunkParent;
		private Dictionary<int, int> _assoCheckpointWithProjectileDamage;
		private int _side;

		#region Unity Lifecycle

		private void Start()
		{
			_player = GameObject.FindGameObjectWithTag("Player");
			if (_player == null)
				throw new Exception("Player not found");

			InitEvents();
			_playerTransform = _player.transform;

			_assoCheckpointWithProjectileDamage = _ennemySettings.LstDistanceCheckpoints
				.Zip(_ennemySettings.LstProjectileDamageByDistance, (key, value) => new { key, value })
				.ToDictionary(x => x.key, x => x.value);
		}

		private void Update()
		{
			Vector3 dirToCamera = Camera.main.transform.position - transform.position;
			dirToCamera.y = 0f;
			_ennemyPivot.transform.localRotation = Quaternion.LookRotation(dirToCamera);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				_patternRoutine ??= StartCoroutine(EnnemyPattern(_side));
			}
		}

		private void OnDisable()
		{
			if (_patternRoutine != null)
			{
				StopCoroutine(_patternRoutine);
				_patternRoutine = null;
			}
		}

		private void OnDestroy()
		{
			RevokeEvents();
		}

		#endregion Unity Lifecycle

		public void SetParent(Chunk chunk)
			=> _chunkParent = chunk;

		public void StartLiving(int side)
		{
			foreach (SkinnedMeshRenderer renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
				renderer.enabled = false;

			_ennemyPivot.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			_side = side;
		}

		private IEnumerator EnnemyPattern(int side)
		{
			_animator.SetBool("IsSideLeft", side == 0);
			_animator.SetTrigger("Spawn");

			while (_isPlaying)
			{
				_animator.SetTrigger("Attack");
				yield return new WaitForSeconds(_ennemySettings.ThrowingInterval);
			}
		}

		public void ThrowProjectile()
		{
			if (this.transform.position.z < _player.transform.position.z)
				return;

			if (_playerTransform == null)
				_playerTransform = _player.transform;

			Vector3 position = _playerTransform.position;

			ProjectileController projectile = Instantiate(RandomisationHelper.GetRandomItemFromList(_lstProjectile), this.transform);
			projectile.transform.position = _projectileSpawnPoint.position;
			projectile.LaunchTowards(position, _ennemySettings.ProjectileSpeed, _ennemySettings.ProjectileLifetime, GetProjectileDamage());
		}

		private int GetProjectileDamage()
		{
			if (_assoCheckpointWithProjectileDamage is null)
			{
				_assoCheckpointWithProjectileDamage = _ennemySettings.LstDistanceCheckpoints
					.Zip(_ennemySettings.LstProjectileDamageByDistance, (key, value) => new { key, value })
					.ToDictionary(x => x.key, x => x.value);
			}

			if (StatsController.Score > _assoCheckpointWithProjectileDamage.Keys.Last())
				return _assoCheckpointWithProjectileDamage.Values.Last();

			int lastKey = _assoCheckpointWithProjectileDamage.Keys.First();
			foreach (int score in _assoCheckpointWithProjectileDamage.Keys)
			{
				if (StatsController.Score > score)
				{
					lastKey = score;
					continue;
				}

				return _assoCheckpointWithProjectileDamage[lastKey];
			}

			throw new UnityException("Projectile damages can't be null or less or equal to 0");
		}

		#region Unity Events

		private void InitEvents()
		{
			UnityEvents.Instance.GameOver.AddListener(() => _isPlaying = false);
			UnityEvents.InteractibleDestroyed += this.ReturnToPool;
		}

		private void RevokeEvents()
		{
			UnityEvents.Instance.GameOver.RemoveListener(() => _isPlaying = false);
			UnityEvents.InteractibleDestroyed -= this.ReturnToPool;
		}

		protected void ReturnToPool(Chunk chunkParent)
		{
			if (_chunkParent != chunkParent)
				return;

			if (_pool == null)
			{
				Debug.LogError("Obstacle should return to pool but it is null !");
				return;
			}

			if (_patternRoutine != null)
				StopCoroutine(_patternRoutine);
			_pool.Release(this.gameObject);
		}

		#endregion Unity Events

		public void SetActive()
		{
			foreach (SkinnedMeshRenderer renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
				renderer.enabled = true;
		}
	}
}