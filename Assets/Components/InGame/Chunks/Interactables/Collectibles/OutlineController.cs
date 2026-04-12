using UnityEngine;

namespace Assets.Components.InGame.Chunks.Interactables.Collectibles
{
	[RequireComponent(typeof(MeshFilter))]
	public class OutlineController : MonoBehaviour
	{
		[SerializeField] private Color _outlineColor = Color.white;
		[SerializeField] private float _outlineWidth = 0.05f;
		[SerializeField] private Shader _outlineShader;

		private GameObject _outlineMesh;

		private void Start()
		{
			if (_outlineShader == null)
			{
				Debug.LogError("[OutlineController] Outline shader non assigné !");
				return;
			}

			_outlineMesh = new GameObject("Outline");
			_outlineMesh.transform.SetParent(transform, false);

			MeshFilter mf = _outlineMesh.AddComponent<MeshFilter>();
			mf.mesh = GetComponent<MeshFilter>().mesh;

			MeshRenderer mr = _outlineMesh.AddComponent<MeshRenderer>();
			Material outlineMat = new Material(_outlineShader);
			outlineMat.SetColor("_OutlineColor", _outlineColor);
			outlineMat.SetFloat("_OutlineWidth", _outlineWidth);
			mr.material = outlineMat;
		}

		private void OnDestroy()
		{
			if (_outlineMesh != null)
				Destroy(_outlineMesh);
		}
	}
}