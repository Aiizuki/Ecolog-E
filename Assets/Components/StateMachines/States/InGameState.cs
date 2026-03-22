using Assets.Components.Game;
using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Components.StateMachines.States
{
	public class InGameState : State
	{
		public InGameState(StateMachine stateMachine) : base(stateMachine)
		{
		}

		// The timer since the game started in seconds.
		public int Timer => Mathf.RoundToInt(_timer);

		private float _timer;

		public override void Enter()
		{
			UnityEvents.Instance.GameOverEvent.AddListener(HandleGameOver);
			_timer = StatsController.GetScore();

			if (!newGame)
				UnityEvents.Instance.GameResumeEvent.Invoke();
		}

		public override void Update()
		{
			_timer += Time.deltaTime;
		}

		public override void Exit()
		{
			StatsController.SetScore(Mathf.FloorToInt(_timer));
		}

		private void HandleGameOver()
		{
			GameOverState gameOverState = new GameOverState(stateMachine);
			stateMachine.ChangeState(gameOverState);
		}
	}
}
