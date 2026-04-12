using UnityEngine;

/// <summary>
/// Creates an outline on attached meshRenderers
/// </summary>
[RequireComponent(typeof(MeshFilter))]
public class OutlineController : MonoBehaviour
{
	[SerializeField] private Color _outlineColor = Color.white;
	[SerializeField] private float _outlineWidth = 0.05f;

	private GameObject _outlineMesh;

	private void Start()
	{
		// Crée un duplicata du mesh juste pour l'outline
		_outlineMesh = new GameObject("Outline");
		_outlineMesh.transform.SetParent(transform, false);

		MeshFilter mf = _outlineMesh.AddComponent<MeshFilter>();
		mf.mesh = GetComponent<MeshFilter>().mesh;

		MeshRenderer mr = _outlineMesh.AddComponent<MeshRenderer>();
		Material outlineMat = new Material(Shader.Find("Custom/OutlineOnly"));
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