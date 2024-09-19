using System;
using System.Collections.Generic;
using ModestTree;

namespace Common.StateMachine
{
    public class InternalState : IInternalState
    {
        private readonly IState _state;
        private readonly List<Type> _nextStates;

        public InternalState(IState state)
        {
            _state = state;
            _nextStates = new List<Type>();
        }
        public IInternalState GoesTo(Type nextStateEnum)
        {
            _nextStates.Add(nextStateEnum);
            return this;
        }
        public IState State => _state;
        public bool IsNextState() => !_nextStates.IsEmpty();
        public bool IsNextState(Type state) => _nextStates.Contains(state);
        public Type NextState()
        {
            if (!IsNextState()) throw new Exception("Next state is missing");
            return _nextStates[0];
        }
    }
}