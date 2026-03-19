using Assets.Components.Game.Obstacle;
using Assets.Scripts.Core;
using Assets.Scripts.Helpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Components.ObstacleGenerator
{
    public class Chunk : MonoBehaviour
    {
        public ChunkSettings _chunkSettings;
        public bool IsDefinedInScene = false;

        [SerializeField] private List<GameObject> _lanes;
        private List<GameObject> _lanesWithObstacle;

        #region Unity Lifecycle

        private void Start()
        {
            if(IsDefinedInScene)
                UnityEvents.Instance.GenerateNewObstacles.Invoke(this);
        }

        private void Update()
        {
            transform.Translate(_chunkSettings.TranslationSpeed * Time.deltaTime * Vector3.back);
            if (IsBehindPlayer())
            {
                if(IsDefinedInScene)
                    Destroy(gameObject);

                _lanesWithObstacle = null;
                
                UnityEvents.Instance.ObstacleDestroyed.Invoke(this);
                UnityEvents.Instance.ChunkDestroyed.Invoke(this);
            }
        }

        #endregion Unity Lifecycle

        #region Public Methods

        public bool IsBehindPlayer()
            => _chunkSettings.DeadZoneZIndex >= transform.position.z;

        public void Spawn(Vector3 position)
        {
            transform.position = position;
            UnityEvents.Instance.GenerateNewObstacles.Invoke(this);
        }

        public void GiveObstacle(List<Obstacle> lstObstacle)
        {
            _lanesWithObstacle ??= new();

            foreach (Obstacle obstacle in lstObstacle)
            {
                if (_lanes.Count == _lanesWithObstacle.Count)
                {
                    Debug.LogWarning("There is more obstacle to place than lanes available !");
                    return;
                }

                GameObject lane = RandomisationHelper.GetRandomItemFromList(_lanes.FindAll(l => !_lanesWithObstacle.Contains(l)).ToList());
                obstacle.transform.SetPositionAndRotation(new Vector3(lane.transform.position.x, lane.transform.position.y, transform.position.z), Quaternion.identity);
                obstacle.transform.SetParent(this.transform);
                obstacle.gameObject.SetActive(true);
                _lanesWithObstacle.Add(lane);
            }
        }

        public int GetNbLanes()
            => _lanes.Count;

        #endregion Public Methods
    }
}