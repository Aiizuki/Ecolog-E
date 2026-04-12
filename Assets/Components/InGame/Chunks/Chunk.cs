using Assets.Components.InGame.Chunks;
using Assets.Components.InGame.Chunks.Interactables;
using Assets.Components.InGame.Chunks.Interactables.Collectibles;
using Assets.Components.InGame.Chunks.Interactables.Obstacles;
using Assets.Components.InGame.Chunks.Lanes;
using Assets.Components.InGame.Ennemy;
using Assets.Components.Singletons;
using Assets.Scripts.Helpers;
using Assets.Settings.Chunks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Component = Assets.Components.InGame.Chunks.Interactables.Collectibles.Component;

namespace Assets.Components.Game.Chunks
{
	/// <summary>
	/// Represents a Chunk in the game
	/// A chunk contains X <see cref="Lane"/> (defined in <see cref="_lanes"/>), and on each lanes we can have multiple <see cref="AInteractable"/>
	/// The procedural generation is made chunk by chunk
	/// </summary>
	public class Chunk : MonoBehaviour
	{
		[Header("Chunk Config")]
		[SerializeField] private ChunkSettings _chunkSettings;
		[SerializeField] private List<Lane> _lanes;
		[SerializeField] private float _length;
		public bool IsDefinedInScene = false;
		public bool IgnoreEnnemySpawn = false;

		[Header("Collectibles Settings")]
		[Tooltip("When tracing a collectible path, defines the chances that the n+1 collectible is placed on another lane")][SerializeField] private float derivationPourcent = 0.4f;

		[Header("Debug")]
		[SerializeField] private bool _debug = true;
		[SerializeField] private bool _debugObstacleGeneration = true;
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
				UnityEvents.GenerateNewInteractibles.Invoke(this);

			UnityEvents.Instance.GameOver.AddListener(Stop);
		}

		private void OnDisable()
		{
			if (_lanes == null)
				return;

			foreach (Lane lane in _lanes)
			{
				AInteractable[] children = lane.GetComponentsInChildren<AInteractable>(includeInactive: true);
				if (children?.Length > 0)
				{
					foreach (AInteractable interactable in children)
					{
						if (interactable == null || interactable.gameObject == null || !interactable.gameObject)
							continue;

						interactable._pool.Release(interactable.gameObject);
					}
				}
			}
			_matrice.Clear();
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

				UnityEvents.InteractibleDestroyed.Invoke(this);
				UnityEvents.ChunkDestroyed.Invoke(this);
			}
		}

		private void OnDestroy()
		{
			UnityEvents.Instance.GameOver.RemoveListener(Stop);
		}

		#endregion Unity Lifecycle

		public bool IsBehindPlayer()
			=> _chunkSettings.DeadZoneZIndex >= transform.position.z;

		public void Spawn(Vector3 position, GameObject _chunkParent)
		{
			transform.position = position;
			transform.SetParent(_chunkParent.transform, true);

			UnityEvents.GenerateNewInteractibles.Invoke(this);
		}

		#region Lane Generation

		/// <summary>
		/// Generates a list of <see cref="AInteractable"/> in lanes
		/// </summary>
		public void GenerateInLanes(List<AInteractable> lstInteractables)
		{
			List<Obstacle> obstacles = lstInteractables.OfType<Obstacle>().ToList();
			List<Collectible> collectibles = lstInteractables.OfType<Collectible>().ToList();
			List<Component> lstComponents = lstInteractables.OfType<Component>().ToList();

			GenerateObstacles(obstacles);
			GenerateCollectiblePath(collectibles);
			GenerateComponents(lstComponents);
		}

#nullable enable
		public void SpawnEnnemies(EnnemyController? ennemy)
		{
			if (ennemy == null)
				return;
			else if (IgnoreEnnemySpawn)
				ennemy._pool.Release(ennemy.gameObject);

			List<Lane> lstSideLanes = new() { _lanes.First(), _lanes.Last() };
			Lane sideLane = RandomisationHelper.GetRandomItemFromList(lstSideLanes);

			sideLane.SpawnEnnemy(ennemy);
		}
#nullable disable

		private void GenerateObstacles(List<Obstacle> lstObstacles)
		{
			int obstacleIndex = 0;
			int step = Mathf.RoundToInt(GetDistanceBetweenObstacles());

			if (_debugObstacleGeneration)
				Debug.Log($"[Chunk] GenerateObstacles — step={step}, NbRows={_matrice.NbRows}, obstacles={lstObstacles.Count}");

			for (int row = _chunkSettings.MinDistanceBeforeObstacleSpawn;
				 row < _matrice.NbRows && obstacleIndex + 1 < lstObstacles.Count;
				 row += step)
			{
				List<Lane> freeLanes = _lanes
					.Where(l => _matrice.Get(row, _lanes.IndexOf(l)) != 1)
					.ToList();

				if (_debugObstacleGeneration)
					Debug.Log($"[Chunk] row={row}, freeLanes={freeLanes.Count}");

				if (freeLanes.Count < 2)
				{
					if (_debugObstacleGeneration)
						Debug.Log($"[Chunk] row={row} skip — not enought free lane");
					continue;
				}

				Lane laneA = RandomisationHelper.GetRandomItemFromList(freeLanes);
				Lane laneB = RandomisationHelper.GetRandomItemFromList(freeLanes, exclude: laneA);

				int idxA = _lanes.IndexOf(laneA);
				int idxB = _lanes.IndexOf(laneB);

				if (_debugObstacleGeneration)
					Debug.Log($"[Chunk] Spawn obstacles — row={row}, idxA={idxA}, idxB={idxB}");

				_matrice.Set(row, idxA, 1);
				_matrice.Set(row, idxB, 1);

				laneA.SpawnInteractible(lstObstacles[obstacleIndex], row);
				laneB.SpawnInteractible(lstObstacles[obstacleIndex + 1], row);
				obstacleIndex += 2;
			}

			for (int i = obstacleIndex; i < lstObstacles.Count; i++)
				lstObstacles[i]._pool.Release(lstObstacles[i].gameObject);
		}

		/// <summary>
		/// Generates every collectible as a path in the chunk
		/// </summary>
		/// <remarks> "Path" means that the n+1 collectible cords follows the n one, but can be on a different lane</remarks>
		private void GenerateCollectiblePath(List<Collectible> lstCollectibles)
		{
			if (lstCollectibles.Count == 0)
				return;

			int collectibleIndex = 0;
			int obstacleMargin = Mathf.RoundToInt(7f);
			int collectibleStep = Mathf.RoundToInt(5f);

			Lane currentLane = RandomisationHelper.GetRandomItemFromList(_lanes);

			for (int row = _chunkSettings.MinDistanceBeforeCollectibleSpawn; row < _matrice.NbRows && collectibleIndex < lstCollectibles.Count; row++)
			{
				int laneIdx = _lanes.IndexOf(currentLane);

				if (!_matrice.IsFreeInRangeOfType(row, laneIdx, obstacleMargin, 1))
				{
					Lane adjacent = GetAdjacentFreeLane(row, currentLane, obstacleMargin);
					if (adjacent == null) continue;
					currentLane = adjacent;
					laneIdx = _lanes.IndexOf(currentLane);
				}

				if (!_matrice.IsFreeInRangeOfType(row, laneIdx, collectibleStep, 2))
					continue;

				_matrice.Set(row, laneIdx, 2);
				currentLane.SpawnInteractible(lstCollectibles[collectibleIndex], row);
				collectibleIndex++;

				if (Random.value < derivationPourcent)
					currentLane = RandomisationHelper.GetRandomItemFromList(_lanes, exclude: currentLane);
			}

			for (int i = collectibleIndex; i < lstCollectibles.Count; i++)
				lstCollectibles[i]._pool.Release(lstCollectibles[i].gameObject);
		}

		/// <summary>
		/// Generates Components in the chunk at the farthest coords from collectibles
		/// </summary>
		/// <remarks> Due to the game design document, <paramref name="lstComponents"/> will always contain only one item (we keep the list in cases were the GDD change)</remarks>
		private void GenerateComponents(List<Component> lstComponents)
		{
			if (lstComponents.Count == 0)
				return;

			if (_debug)
				_matrice.DebugPrint();

			foreach (Component component in lstComponents)
			{
				(int row, int col) = _matrice.GetFarthestPositionFromCollectibles(_chunkSettings.MinDistanceBeforeCollectibleSpawn);
				Debug.Log($"[Component] Placement : row={row}, col={col}, valeur matrice={_matrice.Get(row, col)}");

				if (row == -1 || col == -1)
				{
					if (_debug)
						Debug.Log("Can't place component in chunk");
					component._pool.Release(component.gameObject);
					continue;
				}

				_matrice.Set(row, col, 3);
				_lanes[col].SpawnInteractible(component, row, true);
			}
		}

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

			throw new UnityException("Distance between obstacles can't be null or less or equal to 0");
		}

		private int GetMaxObstaclePerLane(bool getMaxNb = false)
		{
			float distanceBetweenObstacles = GetDistanceBetweenObstacles(getMaxNb);
			return Mathf.FloorToInt(_length / distanceBetweenObstacles);
		}

		#endregion Obstacle Counting

		#region Collectible Counting

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