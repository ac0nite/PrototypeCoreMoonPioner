using Common.StateMachine;
using UnityEngine;
using Zenject;

namespace Application.StateMachine.States
{
    public class ResetGameplayState : IState
    {
        public ResetGameplayState()
        {
            Debug.Log("ResetGameplayState");
        }
        public void OnEnter()
        {
            Debug.Log("Enter ResetGameplayState");
        }

        public void OnExit()
        {
            Debug.Log("Exit ResetGameplayState");
        }
        
        #region FACTORY

        public class Factory : PlaceholderFactory<IState>
        {
        }

        #endregion
    }
}