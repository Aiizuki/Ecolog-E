using Assets.Components.InGame.Ennemy;
using UnityEngine;

public class EnnemyAnimationController : MonoBehaviour
{
	[SerializeField] private EnnemyController _ennemyController;

	#region Unity Lifecycle

	void Start()
	{
		InitEvents();
	}

	private void OnDestroy()
	{
		RevokeEvents();
	}

	#endregion Unity Lifecycle

	#region Unity Events

	private void InitEvents()
	{
		//throw new NotImplementedException();
	}

	private void RevokeEvents()
	{
		//throw new NotImplementedException();
	}

	#endregion Unity Events

	// Called in the animator (attack clip) to match the projectile spawn with the throw animation
	public void ThrowProjectile()
		=> _ennemyController.ThrowProjectile();

	public void SetActive()
		=> _ennemyController.SetActive();
}
