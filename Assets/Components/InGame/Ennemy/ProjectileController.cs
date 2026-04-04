using UnityEngine;

namespace Assets.Components.InGame.Ennemy
{
	public class ProjectileController : MonoBehaviour
	{
		private Vector3 _targetPosition;

		private float _timer;
		private float _speed;
		private float _lifetime;

		public void LaunchTowards(Vector3 position, float speed, float lifetime)
		{
			_targetPosition = position;
			_speed = speed;
			_lifetime = lifetime;
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

		#endregion Unity Lifecycle
	}
}