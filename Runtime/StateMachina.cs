using System;
using System.Collections.Generic;
using UnityEngine;

namespace EFES.StateMachina
{
    public class StateMachina
    {
        public bool EnableDebug = false;
        
        private const int k_ExpediteStateLimit = 10;
        private readonly Dictionary<int, IState> m_StateById = new();
        private readonly List<int> m_ExpeditedStates = new();
        private IState m_CurrState;
        private bool m_IsRunning;

        public interface IState
        {
            public void StateInit(StateMachina stateMachine);
            public void StateStart();
            public void StateUpdate();
            public void StateEnd();
        }

        /// <summary>
        /// This Update must be called manually by the State machine.
        /// </summary>
        public void StateUpdate()
        {
            if (m_IsRunning)
            {
                m_ExpeditedStates.Clear();
                m_CurrState.StateUpdate();
            }
        }

        /// <summary>
        /// Assigns a IState class to the State Machine.
        /// </summary>
        /// <param name="state">A class that uses the IState interface.</param>
        /// <param name="enumId">Enum that contains a unique int value to represent the State ID.</param>
        /// <typeparam name="T">The class type to be created as a state.</typeparam>
        public void AssignState<T>(T state, Enum enumId) where T : class, IState
        {
            AssignState(state, Convert.ToInt32(enumId));
        }

        /// <summary>
        /// Assigns a IState class to the State Machine.
        /// </summary>
        /// <param name="state">A class that uses the IState interface.</param>
        /// <param name="id">Unique int value to represent the State ID.</param>
        /// <typeparam name="T">The class type to be created as a state.</typeparam>
        public void AssignState<T>(T state, int id) where T : class, IState
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
            state.StateInit(this);
            m_StateById[id] = state;

            // // If first assigned state then set as starting state
            // if (m_StateById.Count == 1)
            // {
            //     SetState(id);
            // }
        }

        // Start State Machine
        public void StartStateMachine(Enum startingStateEnumId)
        {
            StartStateMachine(Convert.ToInt32(startingStateEnumId));
        }
        
        public void StartStateMachine(int startingStateId)
        {
            RunStateMachine();
            SetState(startingStateId, true);
        }
        
        // Run State Machine
        public void RunStateMachine()
        {
            m_IsRunning = true;
        }

        // Pause State Machine
        public void PauseStateMachine()
        {
            m_IsRunning = false;
        }

        public void SetState(Enum enumId, bool executeStateUpdateImmidiately = false, bool force = false)
        {
            SetState(Convert.ToInt32(enumId), executeStateUpdateImmidiately, force);
        }

        public void SetState(int id, bool executeStateUpdateImmidiately = false, bool force = false)
        {
            if (!m_StateById.TryGetValue(id, out IState state))
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
                m_CurrState?.StateEnd();
                m_CurrState = state;
                m_CurrState.StateStart();
            }

            if (executeStateUpdateImmidiately)
            {
                if (m_ExpeditedStates.Count <= k_ExpediteStateLimit)
                {
                    if (m_IsRunning)
                    {
                        m_ExpeditedStates.Add(id);
                        m_CurrState.StateUpdate();
                    }
                    else
                    {
                        Debug.LogWarning($"Can't execute state `{m_CurrState}` this frame, because State Machine is not running.");
                    }
                }
                else
                {
                    Debug.LogError(
                        $"Expedite state limit reached. State ID stack: <color=red>{String.Join(", ", m_ExpeditedStates.ToArray())}</color>");
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