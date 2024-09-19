using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.StateMachine
{
    public class BaseStateMachine : IStateMachine
    {
        private Type _currentStateType;
        private Dictionary<Type, IInternalState> _states = new ();

        public Type CurrentStateType => _currentStateType;

        public InternalState Register(IState state)
        {
            var internalState = new InternalState(state);
            _states.Add(state.GetType(), internalState);
            
            return internalState;
        }

        public void NextState()
        {
            var internalState = _states[_currentStateType];
            if (!internalState.IsNextState())
            {
                Debug.LogWarning($"[FSM] Next state is empty! Current: [{_currentStateType}]");
                return;
            }
            
            internalState.State.OnExit();

            _currentStateType = internalState.NextState();
            _states[_currentStateType].State.OnEnter();
        }

        public void NextState(Type type)
        {
            var internalState = _states[_currentStateType];
            if (!internalState.IsNextState(type))
            {
                Debug.LogWarning($"[FSM] Next state is missing! Current: [{type}]");
                return;
            }
            
            internalState.State.OnExit();
            _currentStateType = type;
            _states[_currentStateType].State.OnEnter();
        }

        public void ForceNextState(Type type)
        {
            if (!_states.ContainsKey(type))
            {
                Debug.LogWarning($"[FSM] Force next state is missing [{type}]!");
                return;
            }
            
            var internalState = _states[type];
            internalState.State.OnExit();
            _currentStateType = type;
            _states[type].State.OnEnter();
        }

        public void Run(Type type)
        {
            _currentStateType = type;
            _states[_currentStateType].State.OnEnter();
        }
    }
}