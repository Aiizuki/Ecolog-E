using Assets.Components.Game;
using Assets.Components.ObstacleGenerator;
using Assets.Scripts.Core;
using Assets.Scripts.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class GameDefilementController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private ChunkSettings _settings;
    [SerializeField] private GameObject _leftLaneStartPoint;
    [SerializeField] private GameObject _middleLaneStartPoint;
    [SerializeField] private GameObject _rightLaneStartPoint;
    [SerializeField] private float _chunkYPos = 0.5f;

    [Header("Components")]
    [SerializeField] private List<GameObject> _lstChunkPrefabs;

    private readonly List<Chunk> _instancedChunks = new();

    #region Unity Lifecycle

    private void Start()
    {
        InitEvents();
        SpawnChunkRow();
        StartCoroutine(ChunkGeneration());
    }

    private void Update()
    {
        UpdateChunks();
    }

    private void OnDestroy()
    {
        RevokeEvents();
    }

    #endregion Unity Lifecycle

    #region Private Helpers

    private IEnumerator ChunkGeneration()
    {
        while (true)
        {
            try
            {
                UnityEvents.Instance.GenerateNewChunks.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in chunk generation coroutine: {ex.Message}");
                yield break;
            }

            yield return new WaitForSeconds(_settings.ChunkGenerationDelay);
        }
    }

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
        AddChunk(_leftLaneStartPoint.transform.position);
        AddChunk(_middleLaneStartPoint.transform.position);
        AddChunk(_rightLaneStartPoint.transform.position);
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
        chunk.Spawn(new Vector3(position.x, _chunkYPos, position.z));
        _instancedChunks.Add(chunk);
    }

    #region UnityEvents

    private void InitEvents()
    {
        UnityEvents.Instance.GenerateNewChunks.AddListener(SpawnChunkRow);
    }

    private void RevokeEvents()
    {
        UnityEvents.Instance.GenerateNewChunks.RemoveListener(SpawnChunkRow);
    }

    #endregion UnityEvents

    #endregion Private Helpers
}