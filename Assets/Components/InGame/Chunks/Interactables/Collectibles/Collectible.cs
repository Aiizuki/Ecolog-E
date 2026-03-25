using Assets.Components.Singletons;
using UnityEngine;

namespace Assets.Components.InGame.Chunks.Interactables.Collectibles
{
	public class Collectible : AInteractable
	{
		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.CompareTag("Player"))
			{
				UnityEvents.Instance.HealthGainEvent.Invoke(null);
				_pool.Release(this.gameObject);
			}
		}
	}
}
