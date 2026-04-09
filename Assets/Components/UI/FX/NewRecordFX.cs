using UnityEngine;

namespace Assets.Components.UI.FX
{
	public class NewRecordFX : MonoBehaviour
	{
		#region Unity Lifecycle

		void OnEnable()
		{
			GetComponent<JiggleText>().PlayJiggle("NEW RECORD!");
		}

		private void OnDisable()
		{
			GetComponent<JiggleText>().Stop();
		}

		#endregion Unity Lifecycle
	}
}