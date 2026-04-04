using Assets.Components.Game.Chunks.Lanes;
using Assets.Components.InGame.Chunks;
using Assets.Components.InGame.Chunks.Interactables;
using Assets.Components.InGame.Chunks.Interactables.Collectibles;
using Assets.Components.InGame.Chunks.Interactables.Obstacles;
using Assets.Components.InGame.Ennemy;
using Assets.Components.Singletons;
using Assets.Scripts.Helpers;
using Assets.Settings.Chunks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Components.Game.Chunks
{
	public class Chunk : MonoBehaviour
	{
		[SerializeField] private ChunkSettings _chunkSettings;
		[SerializeField] private List<Lane> _lanes;
		[SerializeField] private float _length;

		[Header("Collectibles Settings")]
		[Tooltip("When tracing a collectible path, defines the chances that it can switch to another lanes")]
		[SerializeField] private float derivationPourcent = 0.4f;


		[Header("Debug")]
		public bool IsDefinedInScene = false;
		[SerializeField] private ChunkMatrix _matrice;

		private Dictionary<int, float> _assoTimeWithDistance;
		private Dictionary<int, float> _assoCheckpointWithDistanceBetweenEnnemies;
		private bool _gameOver = false;

		#region Unity Lifecycle

		private void Awake()
		{
			if (_chunkSettings.LstTimeCheckpoints.Count != _chunkSettings.LstDitancesBetweenTwoObstacles.Count)
				throw new UnityException("Checkpoints and DistancePerCheckpoint lists must have the same Length. Please fix the ChunkSetting");

			_assoTimeWithDistance = _chunkSettings.LstTimeCheckpoints
				.Zip(_chunkSettings.LstDitancesBetweenTwoObstacles, (key, value) => new { key, value })
				.ToDictionary(x => x.key, x => x.value);

			_assoCheckpointWithDistanceBetweenEnnemies = _chunkSettings.LstDistanceCheckpoints
				.Zip(_chunkSettings.LstDitancesBetweenTwoEnnemies, (key, value) => new { key, value })
				.ToDictionary(x => x.key, x => x.value);
		}

		private void OnEnable()
		{
			_matrice = new ChunkMatrix();
			_matrice.Init(Mathf.FloorToInt(_length), GetNbLanes());
		}

		private void Start()
		{
			_gameOver = false;
			if (IsDefinedInScene)
				UnityEvents.Instance.GenerateNewInteractibles.Invoke(this);

			UnityEvents.Instance.GameOverEvent.AddListener(Stop);
		}

		private void OnDisable()
		{
			_matrice?.Clear();
		}

		private void Update()
		{
			if (_gameOver)
				return;

			transform.Translate(ClockManager.GetSpeed() * Time.deltaTime * Vector3.back);
			if (IsBehindPlayer())
			{
				if (IsDefinedInScene)
					Destroy(gameObject);

				UnityEvents.Instance.InteractibleDestroyedEvent.Invoke(this);
				UnityEvents.Instance.ChunkDestroyedEvent.Invoke(this);
			}
		}

		private void OnDestroy()
		{
			UnityEvents.Instance.GameOverEvent.RemoveListener(Stop);
		}

		#endregion Unity Lifecycle

		public bool IsBehindPlayer()
			=> _chunkSettings.DeadZoneZIndex >= transform.position.z;

		public void Spawn(Vector3 position, GameObject _chunkParent)
		{
			transform.position = position;
			transform.SetParent(_chunkParent.transform, true);

			UnityEvents.Instance.GenerateNewInteractibles.Invoke(this);
		}

		#region Lane Generation

		public void GenerateInLanes(List<AInteractable> lstInteractables)
		{
			List<Obstacle> obstacles = lstInteractables.OfType<Obstacle>().ToList();
			List<Collectible> collectibles = lstInteractables.OfType<Collectible>().ToList();
			// TODO: List<SuperCollectible> superCollectibles = lstInteractables.OfType<SuperCollectible>().ToList();

			GenerateObstacles(obstacles);
			GenerateCollectiblePath(collectibles);
			// TODO: GenerateSuperCollectibles(superCollectibles);
		}

		public void SpawnEnnemies(EnnemyController? ennemy)
		{
			if (ennemy == null)
				return;

			List<Lane> lstSideLanes = new() { _lanes.First(), _lanes.Last() };
			Lane sideLane = RandomisationHelper.GetRandomItemFromList(lstSideLanes);

			sideLane.SpawnEnnemy(ennemy);
		}

		private void GenerateObstacles(List<Obstacle> lstObstacles)
		{
			int obstacleIndex = 0;
			int step = Mathf.RoundToInt(GetDistanceBetweenObstacles());

			for (int row = _chunkSettings.MinDistanceBeforeObstacleSpawn; row < _matrice.NbRows && obstacleIndex + 1 < lstObstacles.Count; row += step)
			{
				Lane laneA = RandomisationHelper.GetRandomItemFromList(_lanes);
				Lane laneB = RandomisationHelper.GetRandomItemFromList(_lanes, exclude: laneA);

				int idxA = _lanes.IndexOf(laneA);
				int idxB = _lanes.IndexOf(laneB);

				_matrice.Set(row, idxA, 1);
				_matrice.Set(row, idxB, 1);

				laneA.SpawnInteractible(lstObstacles[obstacleIndex], row);
				laneB.SpawnInteractible(lstObstacles[obstacleIndex + 1], row);

				obstacleIndex += 2;
			}

			// Release tous les obstacles non placés
			for (int i = obstacleIndex; i < lstObstacles.Count; i++)
				lstObstacles[i]._pool.Release(lstObstacles[i].gameObject);
		}

		private void GenerateCollectiblePath(List<Collectible> lstCollectibles)
		{
			if (lstCollectibles.Count == 0)
				return;

			int collectibleIndex = 0;
			int obstacleMargin = Mathf.RoundToInt(7f);  // 7m autour des obstacles
			int collectibleStep = Mathf.RoundToInt(5f);   // 5m entre chaque collectible

			Lane currentLane = RandomisationHelper.GetRandomItemFromList(_lanes);

			for (int row = _chunkSettings.MinDistanceBeforeCollectibleSpawn; row < _matrice.NbRows && collectibleIndex < lstCollectibles.Count; row++)
			{
				int laneIdx = _lanes.IndexOf(currentLane);

				// Vérifie qu'on est pas trop proche d'un obstacle (valeur 1 dans la matrice)
				if (!_matrice.IsFreeInRangeOfType(row, laneIdx, obstacleMargin, 1))
				{
					// Tente une lane adjacente
					Lane adjacent = GetAdjacentFreeLane(row, currentLane, obstacleMargin);
					if (adjacent == null) continue;
					currentLane = adjacent;
					laneIdx = _lanes.IndexOf(currentLane);
				}

				// Vérifie qu'on est pas trop proche d'un autre collectible (valeur 2 dans la matrice)
				if (!_matrice.IsFreeInRangeOfType(row, laneIdx, collectibleStep, 2))
					continue;

				_matrice.Set(row, laneIdx, 2);
				currentLane.SpawnInteractible(lstCollectibles[collectibleIndex], row);
				collectibleIndex++;

				// Dérive aléatoire de lane
				if (Random.value < derivationPourcent)
					currentLane = RandomisationHelper.GetRandomItemFromList(_lanes, exclude: currentLane);
			}

			// Release les collectibles non placés
			for (int i = collectibleIndex; i < lstCollectibles.Count; i++)
				lstCollectibles[i]._pool.Release(lstCollectibles[i].gameObject);
		}

		// Returns an adjacent free lane or null
		private Lane GetAdjacentFreeLane(int row, Lane current, int margin)
		{
			int idx = _lanes.IndexOf(current);

			Lane left = idx > 0 ? _lanes[idx - 1] : null;
			Lane right = idx < _lanes.Count - 1 ? _lanes[idx + 1] : null;

			if (left != null && _matrice.IsFreeInRangeOfType(row, idx - 1, margin, 1)) return left;
			if (right != null && _matrice.IsFreeInRangeOfType(row, idx + 1, margin, 1)) return right;

			return null;
		}

		#endregion Lane Generation

		private int GetNbLanes()
			=> _lanes.Count;

		private void Stop()
			=> _gameOver = true;

		#region Obstacle Counting

		/// <summary>
		/// Determine the number of <see cref="Obstacle"/> we need to assign to a chunk
		/// </summary>
		public int GetNbObstacle()
			=> GetMaxObstaclePerLane() * GetNbLanes();

		private float GetDistanceBetweenObstacles(bool getMaxNb = false)
		{
			if (StatsController.InGameTime > _assoTimeWithDistance.Keys.Last() || getMaxNb)
				return _assoTimeWithDistance.Values.Last();

			int lastKey = _assoTimeWithDistance.Keys.First();
			foreach (int time in _assoTimeWithDistance.Keys)
			{
				if (StatsController.InGameTime > time)
				{
					lastKey = time;
					continue;
				}

				return _assoTimeWithDistance[lastKey];
			}

			throw new UnityException("Distance between obstalces can't be null or less or equal to 0");
		}

		private int GetMaxObstaclePerLane(bool getMaxNb = false)
		{
			float distanceBetweenObstacles = GetDistanceBetweenObstacles(getMaxNb);
			return Mathf.FloorToInt(_length / distanceBetweenObstacles);
		}

		#endregion Obstacle Counting

		#region Collectible Counting

		/// <summary>
		/// Determine the number of <see cref="Collectible"/> we need to assign to a chunk
		/// </summary>
		public int GetNbCollectible()
			=> Mathf.Min(GetMaxCollectiblePerLane(), _chunkSettings.MaxCollectiblePerChunk);

		private float GetDistanceBetweenCollectible()
			=> _chunkSettings.DistanceBetweenCollectibles;

		private int GetMaxCollectiblePerLane()
		{
			float distanceBetweenCollectibles = GetDistanceBetweenCollectible();
			return Mathf.FloorToInt(_length / distanceBetweenCollectibles);
		}

		#endregion Collectible Counting

		#region Ennemy Counting

		public float GetDistanceBetweenEnnemies(bool getMaxNb = false)
		{
			if (StatsController.Score > _assoCheckpointWithDistanceBetweenEnnemies.Keys.Last() || getMaxNb)
				return _assoCheckpointWithDistanceBetweenEnnemies.Values.Last();

			int lastKey = _assoCheckpointWithDistanceBetweenEnnemies.Keys.First();
			foreach (int score in _assoCheckpointWithDistanceBetweenEnnemies.Keys)
			{
				if (StatsController.Score > score)
				{
					lastKey = score;
					continue;
				}

				return _assoCheckpointWithDistanceBetweenEnnemies[lastKey];
			}

			throw new UnityException("Distance between ennemies can't be null or less or equal to 0");
		}

		#endregion Ennemy Counting
	}
}