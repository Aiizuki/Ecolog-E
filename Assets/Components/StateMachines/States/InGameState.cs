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
			UnityEvents.Instance.GameOverEvent.AddListener(HandleGameOver);
			UnityEvents.Instance.PlayRunAnimation.Invoke();

			if (!_newGame)
			{
				UnityEvents.Instance.GameResumeEvent.Invoke();
				_newGame = true;
			}
		}

		public override void Exit()
		{
			UnityEvents.Instance.GameOverEvent.RemoveListener(HandleGameOver);
		}

		private void HandleGameOver()
		{
			GameOverState gameOverState = new GameOverState(stateMachine);
			stateMachine.ChangeState(gameOverState);
		}
	}
}
