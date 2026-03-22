using Assets.Components.StateMachines.States;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Components.StateMachines
{
	public class GameStateController : MonoBehaviour
	{
		private StateMachine _stateMachine;
		private List<State> _lstAvailableStates;

		#region Unity Lifecycle

		private void Start()
		{
			_stateMachine = new StateMachine();
			_lstAvailableStates = new List<State>();

			InGameState initialState = new InGameState(_stateMachine);
			_lstAvailableStates.Add(initialState);
			_stateMachine.ChangeState(initialState);
		}

		private void Update()
		{
			_stateMachine.Update();
		}

		#endregion Unity Lifecycle

		public void ChangeState(Type newState)
		{
			switch (newState)
			{
				case var t when t == typeof(InvincibleState):
					State invincibleState = _lstAvailableStates.FirstOrDefault(s => s is InvincibleState);
					invincibleState ??= new InvincibleState(_stateMachine);
					_stateMachine.ChangeState(invincibleState);
					if (!_lstAvailableStates.Contains(invincibleState))
						_lstAvailableStates.Add(invincibleState);
					break;
				case var t when t == typeof(GameOverState):
					State gameOverState = _lstAvailableStates.FirstOrDefault(s => s is GameOverState);
					gameOverState ??= new GameOverState(_stateMachine);
					_stateMachine.ChangeState(gameOverState);
					if (!_lstAvailableStates.Contains(gameOverState))
						_lstAvailableStates.Add(gameOverState);
					break;
				case var t when t == typeof(InGameState):
					State ingameState = _lstAvailableStates.FirstOrDefault(s => s is InGameState);
					ingameState ??= new InvincibleState(_stateMachine);
					_stateMachine.ChangeState(ingameState);
					if (!_lstAvailableStates.Contains(ingameState))
						_lstAvailableStates.Add(ingameState);
					break;

			}
		}

		public State GetCurrentState()
			=> _stateMachine.CurrentState;
	}
}
