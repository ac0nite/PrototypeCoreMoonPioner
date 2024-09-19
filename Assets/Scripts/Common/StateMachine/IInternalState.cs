using System;

namespace Common.StateMachine
{
    public interface IInternalState
    {
        IInternalState GoesTo(Type nextStateType);
        IState State { get; }
        bool IsNextState();
        bool IsNextState(Type nextStateType);
        Type NextState();
    }
}