using Assets.Components.InGame.Chunks.Interactables;
using Assets.Components.InGame.Ennemy;
using UnityEngine;

namespace Assets.Components.Game.Chunks.Lanes
{
	public class Lane : MonoBehaviour
	{
		public void SpawnInteractible(AInteractable interactible, int position)
		{
			// position en local, centré sur le mesh
			float localZ = -(GetLaneLength() / 2f) + position;

			interactible.transform.SetParent(this.transform, true);
			interactible.transform.localPosition = new Vector3(0f, 0.5f, localZ / transform.localScale.z);
			interactible.transform.localRotation = Quaternion.identity;
			interactible.gameObject.SetActive(true);
		}

		internal void SpawnEnnemy(EnnemyController ennemy)
		{
			// position en local, centré sur le mesh
			float localZ = -(GetLaneLength() / 2f);

			ennemy.transform.SetParent(this.transform, true);
			ennemy.transform.localPosition = new Vector3(0f, 0.5f, localZ / transform.localScale.z);
			ennemy.transform.localRotation = Quaternion.identity;
			ennemy.gameObject.SetActive(true);
			ennemy.SetParent(GetComponentInParent<Chunk>());
			ennemy.StartLiving(); // TODO:  remplacer par un rangeChecker dans un Update dans EnnemyController
		}

		private float GetLaneLength()
		{
			Renderer rend = GetComponent<Renderer>() ?? GetComponentInChildren<Renderer>();
			if (rend == null)
			{
				Debug.LogError($"[Lane] {name} : aucun Renderer trouvé !");
				return 0f;
			}
			return transform.localScale.z * rend.localBounds.size.z;
		}
	}
}
