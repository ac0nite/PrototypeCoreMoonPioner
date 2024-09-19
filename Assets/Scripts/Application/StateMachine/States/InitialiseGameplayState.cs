using Common.StateMachine;
using UnityEngine;
using Zenject;

namespace Application.StateMachine.States
{
    public class InitialiseGameplayState : IState
    {
        public InitialiseGameplayState()
        {
            Debug.Log("InitialiseGameplayState");
        }

        public void OnEnter()
        {
            Debug.Log("Enter InitialiseGameplayState");
        }

        public void OnExit()
        {
            Debug.Log("Exit InitialiseGameplayState");
        }
        
        #region FACTORY

        public class Factory : PlaceholderFactory<IState>
        {
        }

        #endregion
    }
}