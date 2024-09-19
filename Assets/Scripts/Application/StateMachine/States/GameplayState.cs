using Common.StateMachine;
using UnityEngine;
using Zenject;

namespace Application.StateMachine.States
{
    public class GameplayState : IState
    {
        public GameplayState()
        {
            Debug.Log("GameplayState");
        }

        public void OnEnter()
        {
            Debug.Log("Enter GameplayState");
        }

        public void OnExit()
        {
            Debug.Log("Exit GameplayState");
        }
        
        #region FACTORY

        public class Factory : PlaceholderFactory<IState>
        {
        }

        #endregion
    }
}