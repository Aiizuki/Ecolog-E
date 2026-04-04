using Assets.Components.Game.Chunks;
using Assets.Components.InGame.Chunks.Interactables;
using Assets.Components.Singletons;
using Assets.Scripts.Helpers;
using Assets.Settings.GameDefilement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Components.InGame.Ennemy
{
	public class EnnemyController : AInteractable
	{
		[SerializeField] private List<ProjectileController> _lstProjectile;
		[SerializeField] private EnnemySettings _ennemySettings;

		private Transform _playerTransform;
		private Coroutine _patternRoutine;

		private bool _isPlaying = true;
		private Chunk _chunkParent;

		#region Unity Lifecycle

		private new void Start()
		{
			InitEvents();
			_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		}

		private void OnDestroy()
		{
			if (_patternRoutine != null)
				StopCoroutine(_patternRoutine);

			RevokeEvents();
		}

		#endregion Unity Lifecycle

		public void SetParent(Chunk chunk)
			=> _chunkParent = chunk;

		public void StartLiving()
			=> _patternRoutine = StartCoroutine(EnnemyPattern());

		private IEnumerator EnnemyPattern()
		{
			// TODO : rajouter une animation de spawn et une animation de despawn
			// TODO : rajouter un setting lifetime sur l'ennemi (au bout du décompte il despawn)

			if (_playerTransform == null)
				_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

			while (_isPlaying)
			{
				ThrowProjectile(_playerTransform.position);
				yield return new WaitForSeconds(_ennemySettings.ThrowingInterval);
			}
		}

		private void ThrowProjectile(Vector3 position)
		{
			ProjectileController projectile = Instantiate(RandomisationHelper.GetRandomItemFromList(_lstProjectile), transform);
			projectile.LaunchTowards(position, _ennemySettings.ProjectileSpeed, _ennemySettings.ProjectileLifetime);
		}

		#region Unity Events

		private void InitEvents()
		{
			UnityEvents.Instance.GameOverEvent.AddListener(() => _isPlaying = false);
			UnityEvents.Instance.GamePauseEvent.AddListener(() => _isPlaying = false);
			UnityEvents.Instance.GameResumeEvent.AddListener(() => _isPlaying = true);
			UnityEvents.Instance.InteractibleDestroyedEvent.AddListener(this.ReturnToPool);
		}

		private void RevokeEvents()
		{
			UnityEvents.Instance.GameOverEvent.RemoveListener(() => _isPlaying = false);
			UnityEvents.Instance.GamePauseEvent.RemoveListener(() => _isPlaying = false);
			UnityEvents.Instance.GameResumeEvent.AddListener(() => _isPlaying = true);
		}

		protected new void ReturnToPool(Chunk chunkParent)
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
	}
}