using Assets.Components.Game;
using Assets.Components.ObstacleGenerator;
using Assets.Scripts.Core;
using Assets.Scripts.Helpers;
using System.Collections.Generic;
using UnityEngine;

public class GameDefilementController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private ChunkSettings _settings;
    [SerializeField] private Vector3 _leftLaneStartPos;
    [SerializeField] private Vector3 _middleLaneStartPos;
    [SerializeField] private Vector3 _rightLaneStartPos;

    [Header("Components")]
    [SerializeField] private List<GameObject> _lstChunkPrefabs;

    private readonly List<Chunk> _instancedChunks = new();

    #region Unity Lifecycle

    private void Awake()
    {
        UnityEvents.Instance.GenerateNewChunks.AddListener(SpawnChunkRow);
    }

    private void Start()
    {
        SpawnChunkRow();
    }

    private void Update()
    {
        UpdateChunks();
    }

    private void OnDestroy()
    {
        UnityEvents.Instance.GenerateNewChunks.RemoveListener(SpawnChunkRow);
    }

    #endregion Unity Lifecycle

    #region Private Helpers

    private void UpdateChunks()
    {
        List<Chunk> behindChunks = new();

        foreach (Chunk chunk in _instancedChunks)
        {
            if (chunk.IsBehindPlayer())
                behindChunks.Add(chunk);
        }

        foreach (Chunk chunk in behindChunks)
        {
            _instancedChunks.Remove(chunk);
            ObjectPoolManager.Instance.Release(chunk.gameObject);
        }
    }

    private void SpawnChunkRow()
    {
        AddChunk(_leftLaneStartPos);
        AddChunk(_middleLaneStartPos);
        AddChunk(_rightLaneStartPos);
    }

    private void AddChunk(Vector3 position)
    {
        if (_lstChunkPrefabs.Count == 0)
        {
            Debug.LogError("No chunks in pool");
            return;
        }

        GameObject prefab = RandomisationHelper.GetRandomItemFromList(_lstChunkPrefabs);
        GameObject obj = ObjectPoolManager.Instance.Get(prefab);

        if (obj == null)
            return;

        Chunk chunk = obj.GetComponent<Chunk>();
        chunk.Spawn(position);
        _instancedChunks.Add(chunk);
    }

    #endregion Private Helpers
}