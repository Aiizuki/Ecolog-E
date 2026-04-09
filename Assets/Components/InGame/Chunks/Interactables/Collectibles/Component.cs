using Assets.Components.Singletons;
using UnityEngine;

namespace Assets.Components.InGame.Chunks.Interactables.Collectibles
{
	public class Component : AInteractable
	{
		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.CompareTag("Player"))
			{
				UnityEvents.OnComponentCollected.Invoke();
				_pool.Release(this.gameObject);
			}
		}
	}
}
