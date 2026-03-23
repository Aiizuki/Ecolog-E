using Assets.Scripts.Core;

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

			if (!_newGame)
			{
				UnityEvents.Instance.GameResumeEvent.Invoke();
				_newGame = true;
			}
		}

		private void HandleGameOver()
		{
			GameOverState gameOverState = new GameOverState(stateMachine);
			stateMachine.ChangeState(gameOverState);
		}
	}
}
