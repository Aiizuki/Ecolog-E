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
			Debug.Log("Changing state from: " + CurrentState?.GetType().Name + " to: " + newState.GetType().Name);

			CurrentState?.Exit();
			CurrentState = newState;
			CurrentState.Enter();

			UnityEvents.Instance.OnStateChangedEvent?.Invoke(CurrentState);
		}

		public void Update()
		{
			CurrentState?.Update();
		}
	}
}