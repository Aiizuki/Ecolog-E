using UnityEngine;

namespace Assets.Components.ObstacleGenerator
{
    public class Obstacle : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player hit an obstacle!");
                // TODO : add more logic here, such as reducing player health or triggering a game over.
            }
        }
    }
}