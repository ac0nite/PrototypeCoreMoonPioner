using System;

namespace Common.StateMachine
{
    public interface IStateMachine
    {
        public Type CurrentStateType { get; }
        InternalState Register(IState state);
        void NextState();
        void NextState(Type nextStateType);
        void ForceNextState(Type nextStateType);
        void Run(Type runStateType);
    }
}