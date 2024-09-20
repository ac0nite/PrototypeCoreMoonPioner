using System;
using Application.StateMachine.States;
using Common.StateMachine;
using Zenject;

namespace Application.StateMachine
{
    public class GameplayStateMachine : BaseStateMachine, IInitializable, IDisposable
    {
        private readonly SignalBus _signals;
        private readonly InitialiseGameplayState.Factory _initialiseStateFactory;
        private readonly GameplayState.Factory _gameplayStateFactory;
        private readonly ResetGameplayState.Factory _resetGameplayStateFactory;

        public GameplayStateMachine(
            SignalBus signals,
            InitialiseGameplayState.Factory initialiseStateFactory,
            GameplayState.Factory gameplayStateFactory,
            ResetGameplayState.Factory resetGameplayStateFactory)
        {
            _signals = signals;
            _initialiseStateFactory = initialiseStateFactory;
            _gameplayStateFactory = gameplayStateFactory;
            _resetGameplayStateFactory = resetGameplayStateFactory;
            _signals.Subscribe<Signals.NextState>(NextStateChangeHandler);
        }
        
        public void Initialize()
        {
            Register(_initialiseStateFactory.Create()).GoesTo(typeof(GameplayState));
            Register(_gameplayStateFactory.Create()).GoesTo(typeof(ResetGameplayState));
            Register(_resetGameplayStateFactory.Create()).GoesTo(typeof(GameplayState));
            
            Run(typeof(InitialiseGameplayState));
        }

        private void NextStateChangeHandler(Signals.NextState arg) => NextState(arg.NextStateType);

        public void Dispose() => _signals.TryUnsubscribe<Signals.NextState>(NextStateChangeHandler);
        
        #region SIGNALS

        public static class Signals
        {
            public class NextState
            {
                public Type NextStateType { get; private set; }
                public NextState(Type nextStateType)
                {
                    NextStateType = nextStateType;
                }
            }
        }

        #endregion
    }
}