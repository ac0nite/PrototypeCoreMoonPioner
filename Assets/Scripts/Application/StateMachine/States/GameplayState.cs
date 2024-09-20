using Common.StateMachine;
using Core.Input;
using UnityEngine;
using Zenject;

namespace Application.StateMachine.States
{
    public class GameplayState : IState
    {
        private readonly IInputHandler _inputHandler;

        public GameplayState(IInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
        }

        public void OnEnter()
        {
            _inputHandler.Lock = false;
        }

        public void OnExit()
        {
            _inputHandler.Lock = true;
        }
        
        #region FACTORY

        public class Factory : PlaceholderFactory<IState>
        {
        }

        #endregion
    }
}