using Common.StateMachine;
using Gameplay.Locations;
using UnityEngine;
using Zenject;

namespace Application.StateMachine.States
{
    public class InitialiseGameplayState : IState
    {
        private readonly LoadLocations _loaderLocations;
        private readonly SignalBus _signals;

        public InitialiseGameplayState(
            LoadLocations loaderLocations,
            SignalBus signals)
        {
            _loaderLocations = loaderLocations;
            _signals = signals;
        }

        public void OnEnter()
        {
            _loaderLocations.Load(0);
            _signals.TryFire(new GameplayStateMachine.Signals.NextState(typeof(GameplayState)));
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