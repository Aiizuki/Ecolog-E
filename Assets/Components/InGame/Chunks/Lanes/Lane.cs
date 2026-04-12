using Assets.Components.Game.Chunks;
using Assets.Components.InGame.Chunks.Interactables;
using Assets.Components.InGame.Ennemy;
using UnityEngine;

namespace Assets.Components.InGame.Chunks.Lanes
{
	public class Lane : MonoBehaviour
	{
		[SerializeField] private int side; //0 = left, 1= middle, 2=right
		[SerializeField] private Transform spawnPos;

		/// <summary>
		/// Spawns an interacible at a position on a lane
		/// </summary>
		/// <remarks> We use localPosition and rotation because the lane is a plane, so the mesh multiplies per 10 every coords</remarks>
		/// <remarks><paramref name="multiply"/> Is used for spawning <see cref="Component"/>, as it coords are defined in <see cref="ChunkMatrix"/>, which scales with the lane</remarks>
		public void SpawnInteractible(AInteractable interactible, int position, bool multiply = false)
		{
			float localZ = -(GetLaneLength() / 2f) + (multiply ? position * 10 : position);

			interactible.transform.SetParent(this.transform, true);
			interactible.transform.localPosition = new Vector3(0f, interactible.transform.localPosition.y + 0.5f, localZ / transform.localScale.z);
			interactible.transform.localRotation = Quaternion.identity;
			interactible.gameObject.SetActive(true);
		}

		/// <summary>
		/// Spawns an interacible at a position on a lane
		/// </summary>
		/// <remarks> We use localPosition and rotation because the lane is a plane, so the mesh multiplies per 10 every coords</remarks>
		internal void SpawnEnnemy(EnnemyController ennemy)
		{
			ennemy.transform.SetParent(this.transform, true);

			Vector3 localPos = this.transform.InverseTransformPoint(spawnPos.position);
			ennemy.transform.localPosition = localPos;
			ennemy.SetParent(GetComponentInParent<Chunk>());
			ennemy.StartLiving(side);
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
