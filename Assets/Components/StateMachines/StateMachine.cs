using Assets.Components.Singletons;
using Assets.Components.StateMachines.States;
using UnityEngine;

namespace Assets.Components.StateMachines
{
	public class StateMachine
	{
		public State CurrentState;

		public void ChangeState(State newState)
		{
			if (CurrentState is GameOverState)
				return;

			Debug.Log("Changing state from: " + CurrentState?.GetType().Name + " to: " + newState.GetType().Name);

			if (CurrentState != null)
				CurrentState.Exit();

			CurrentState = newState;
			CurrentState.Enter();

			UnityEvents.StateChanged.Invoke(CurrentState);
		}

		public void Update()
		{
			CurrentState?.Update();
		}
	}
}