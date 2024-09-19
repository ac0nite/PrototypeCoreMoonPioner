using System;
using Application.StateMachine.States;
using Common.StateMachine;
using Zenject;

namespace Application.StateMachine
{
    public class GameplayStateMachine : BaseStateMachine, IInitializable, IDisposable
    {
        private readonly SignalBus _signals;

        public GameplayStateMachine(
            SignalBus signals,
            InitialiseGameplayState.Factory initialiseStateFactory,
            GameplayState.Factory gameplayStateFactory,
            ResetGameplayState.Factory resetGameplayStateFactory)
        {
            _signals = signals;

            Register(initialiseStateFactory.Create()).GoesTo(typeof(GameplayState));
            Register(gameplayStateFactory.Create()).GoesTo(typeof(ResetGameplayState));
            Register(resetGameplayStateFactory.Create()).GoesTo(typeof(GameplayState));

            _signals.Subscribe<Signals.NextState>(NextStateChangeHandler);
        }
        
        public void Initialize()
        {
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