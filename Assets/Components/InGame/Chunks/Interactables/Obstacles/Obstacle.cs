using Assets.Components.Singletons;
using UnityEngine;

namespace Assets.Components.InGame.Chunks.Interactables.Obstacles
{
	public class Obstacle : AInteractable
	{
		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.CompareTag("Player"))
			{
				UnityEvents.Instance.HealthLooseEvent.Invoke(null);
				_pool.Release(this.gameObject);
			}
		}
	}
}