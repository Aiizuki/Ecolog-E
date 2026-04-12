using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Components.UI.FX
{
	/// <summary>
	/// Creates a jiggle animation on a text
	/// </summary>
	public class JiggleText : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _label;
		[SerializeField] private float _amplitude = 40f;        // bounce height (px)
		[SerializeField] private float _duration = 0.6f;        // duration per letter (seconds)
		[SerializeField] private float _staggerDelay = 0.04f;   // space between each letter (seconds)
		[SerializeField] private float _maxRotation = 12f;      // max rotation (°)

		private Coroutine _jiggleRoutine;

		public void PlayJiggle(string text)
		{
			_label.text = text;
			_label.ForceMeshUpdate();
			_jiggleRoutine = StartCoroutine(JiggleCoroutine());
		}

		public void Stop()
		{
			if (_jiggleRoutine != null)
				StopCoroutine(_jiggleRoutine);
		}

		private IEnumerator JiggleCoroutine()
		{
			TMP_TextInfo textInfo = _label.textInfo;

			while (true)
			{
				for (int i = 0; i < textInfo.characterCount; i++)
				{
					if (!textInfo.characterInfo[i].isVisible)
						continue;

					StartCoroutine(AnimateLetter(i));
					yield return new WaitForSeconds(_staggerDelay);
				}

				yield return new WaitForSeconds(1.5f);
			}

		}

		private IEnumerator AnimateLetter(int charIndex)
		{
			TMP_TextInfo textInfo = _label.textInfo;
			int meshIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
			int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

			Vector3[] sourceVerts = textInfo.meshInfo[meshIndex].vertices;
			Vector3[] originalVerts = new Vector3[4];
			for (int j = 0; j < 4; j++)
				originalVerts[j] = sourceVerts[vertexIndex + j];

			Vector3 center = (originalVerts[0] + originalVerts[1] + originalVerts[2] + originalVerts[3]) / 4f;

			float elapsed = 0f;

			while (elapsed < _duration)
			{
				elapsed += Time.deltaTime;
				float t = Mathf.Clamp01(elapsed / _duration);

				float eased = EaseOutElastic(t);
				float offsetY = _amplitude * Mathf.Sin(Mathf.PI * t) * (1f - t) * 2f;
				float rotation = _maxRotation * Mathf.Sin(Mathf.PI * t) * (1f - t) * 2f;
				float scaleX = 1f - (1f - eased) * 0.2f;
				float scaleY = 1f + (1f - eased) * 0.6f;

				Quaternion rot = Quaternion.Euler(0f, 0f, rotation);

				for (int j = 0; j < 4; j++)
				{
					Vector3 v = originalVerts[j] - center;
					v.x *= scaleX;
					v.y *= scaleY;
					v = rot * v;
					v += center;
					v.y += offsetY;
					sourceVerts[vertexIndex + j] = v;
				}

				_label.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
				yield return null;
			}

			// Reset aux positions d'origine
			for (int j = 0; j < 4; j++)
				sourceVerts[vertexIndex + j] = originalVerts[j];
			_label.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
		}

		private float EaseOutElastic(float t)
		{
			if (t == 0f || t == 1f)
				return t;

			float c4 = (2f * Mathf.PI) / 3f;
			return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
		}
	}
}