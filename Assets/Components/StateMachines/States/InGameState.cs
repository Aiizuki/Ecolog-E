using Assets.Components.Singletons;

namespace Assets.Components.StateMachines.States
{
	public class InGameState : State
	{
		private bool _newGame;

		public InGameState(StateMachine stateMachine) : base(stateMachine)
		{
			_newGame = false;
		}

		public override void Enter()
		{
			UnityEvents.PlayRunAnimation.Invoke();

			if (!_newGame)
			{
				_newGame = true;
			}
		}
	}
}
