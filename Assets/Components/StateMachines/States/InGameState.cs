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
			UnityEvents.Instance.PlayRunAnimation.Invoke();

			if (!_newGame)
			{
				UnityEvents.Instance.GameResumeEvent.Invoke();
				_newGame = true;
			}
		}
	}
}
