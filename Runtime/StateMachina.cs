using System;
using System.Collections.Generic;
using UnityEngine;

namespace EFES.StateMachina
{
    public class StateMachina
    {
        public bool EnableDebug;

        readonly Dictionary<int, State> m_StateById = new();
        readonly List<int> m_ExpediteStates = new();
        const int k_ExpediteStateLimit = 10;
        State m_CurrState;
        bool m_IsRunning;

        public abstract class State
        {
            public StateMachina StateMachine;

            public virtual void Init()
            {
            }

            public virtual void Start()
            {
            }

            public virtual void Update()
            {
            }

            public virtual void End()
            {
            }
        }

        public virtual void Update()
        {
            if (m_IsRunning)
            {
                m_ExpediteStates.Clear();
                m_CurrState.Update();
            }
        }

        public void AssignState(State state, Enum enumId)
        {
            AssignState(state, Convert.ToInt32(enumId));
        }

        public void AssignState(State state, int id)
        {
            if (state == null)
            {
                Debug.LogError($"Valid owner and state must be passed for id <color=blue>{id}</color>!");
                return;
            }

            // Ensure only one instance of each state
            if (m_StateById.ContainsKey(id))
            {
                Debug.LogError(
                    $"State ID <color=blue>{id}</color> is already in use. Cancelling assignment of <color=blue>{state.GetType().Name}</color>.");
                return;
            }

            // SetState
            state.StateMachine = this;
            state.Init();
            m_StateById[id] = state;

            // If first assigned state then set as starting state
            if (m_StateById.Count == 1)
            {
                SetState(id);
            }
        }

        public void StartStateMachine()
        {
            m_IsRunning = true;
        }

        public void StopStateMachine()
        {
            m_IsRunning = false;
        }

        public void SetState(Enum enumId, bool expediteState = false, bool force = false)
        {
            SetState(Convert.ToInt32(enumId), expediteState, force);
        }

        public void SetState(int id, bool expediteState = false, bool force = false)
        {
            if (!m_StateById.TryGetValue(id, out State state))
            {
                Debug.LogError($"State ID {id} could not be found.");
                return;
            }

            if (EnableDebug)
            {
                Debug.Log($"State: <color=green>{state.GetType()}</color>");
            }

            if (state != m_CurrState || force)
            {
                m_CurrState?.End();
                m_CurrState = state;
                m_CurrState.Start();
            }

            if (expediteState)
            {
                if (m_ExpediteStates.Count <= k_ExpediteStateLimit)
                {
                    m_ExpediteStates.Add(id);
                    m_CurrState.Update();
                }
                else
                {
                    Debug.LogError(
                        $"Expedite state limit reached. State ID stack: <color=red>{String.Join(", ", m_ExpediteStates.ToArray())}</color>");
                }
            }
        }

        public bool StateIdExists(Enum enumId)
        {
            return StateIdExists(Convert.ToInt32(enumId));
        }

        public bool StateIdExists(int id)
        {
            return m_StateById.ContainsKey(id);
        }
    }
}