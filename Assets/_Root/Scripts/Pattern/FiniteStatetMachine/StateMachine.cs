using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMachine
    {
        public State CurrentState { get; private set; }

        public State[] States { get; private set; }

        public void InitStates(params State[] states)
        {
            States = states;
            foreach (var state in States)
            {
                state.StateMachine = this;
            }
        }

        public void ChangeState<T>(object data = null) where T : State
        {
            var state = States.FirstOrDefault(s => s is T);
            ChangeState(state, data);
        }

        void ChangeState(State state, object data = null)
        {
            if (state == CurrentState) return;
            
            var oldState = CurrentState;
            CurrentState = state;
            oldState?.StateExit(state);
            state?.StateEnter(oldState, data);
        }

        public void Tick()
        {
            CurrentState?.StateTick();
        }

        public void FixedTick()
        {
            CurrentState?.StateFixedTick();
        }
    }