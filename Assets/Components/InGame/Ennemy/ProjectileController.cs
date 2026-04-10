using Assets.Components.Singletons;
using UnityEngine;

namespace Assets.Components.InGame.Ennemy
{
	public class ProjectileController : MonoBehaviour
	{
		private Vector3 _targetPosition;

		private float _timer;
		private float _speed;
		private float _lifetime;
		private int _damage;

		public void LaunchTowards(Vector3 position, float speed, float lifetime, int damage)
		{
			_targetPosition = position;
			_speed = speed;
			_lifetime = lifetime;
			_damage = damage;
		}

		#region Unity Lifecycle

		private void Update()
		{
			transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);

			if (transform.position == _targetPosition)
			{
				Destroy(gameObject);
			}

			_timer += Time.deltaTime;
			if (_timer > _lifetime)
			{
				Destroy(gameObject);
			}
		}
		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.CompareTag("Player"))
			{
				UnityEvents.HealthLoose.Invoke(_damage);
				UnityEvents.OnObstacleCollision.Invoke();
				Destroy(this.gameObject);
			}
		}

		#endregion Unity Lifecycle
	}
}