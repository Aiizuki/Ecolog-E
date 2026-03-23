using Assets.Scripts.Core;

namespace Assets.Components.StateMachines.States
{
	public class GameOverState : State
	{
		public GameOverState(StateMachine stateMachine) : base(stateMachine)
		{
		}

		public override void Enter()
		{
			UnityEvents.Instance.GameOverEvent.Invoke();
		}
	}
}
