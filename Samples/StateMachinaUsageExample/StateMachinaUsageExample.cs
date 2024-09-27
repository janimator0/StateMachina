using UnityEngine;

namespace EFES.StateMachina
{
    public class StateMachinaUsageExample : MonoBehaviour
    {
        private readonly StateMachina m_StateMachine = new StateMachina();

        internal enum State
        {
            Idle,
            ButtonDown,
        }

        private void Start()
        {
            m_StateMachine.EnableDebug = true;

            // Assign States
            m_StateMachine.AssignState(this, new StateButtonUp(), State.Idle);
            m_StateMachine.AssignState(this, new StateButtonDown(), State.ButtonDown);

            // Start state machine
            m_StateMachine.StartStateMachine(State.Idle);
        }

        private void Update()
        {
            m_StateMachine.StateUpdate();
        }
    }

    public class StateButtonUp : StateMachina.IState
    {
        private StateMachinaUsageExample m_Owner;
        private StateMachina m_StateMachine;
        private int m_UpdateCount = 0;

        public void StateInit<T>(T owner, StateMachina stateMachine) where T : class
        {
            m_Owner = owner as StateMachinaUsageExample;
            if (m_Owner == null)
            {
                Debug.LogError($"Owner is not of expected type {nameof(StateMachinaUsageExample)}.");
            }

            m_StateMachine = stateMachine;
            Debug.Log($"State Init: {GetType()}, with owner: {m_Owner.GetType()}");
        }

        public void StateStart()
        {
            m_UpdateCount = 0;
            Debug.Log($"State Start: {GetType()} on frame {Time.frameCount}");
        }

        public void StateUpdate()
        {
            m_UpdateCount++;
            if (m_UpdateCount == 1)
            {
                Debug.Log($"First State Update: on frame {Time.frameCount}");
            }

            // If button down detected switch states.
            if (Input.GetKey(KeyCode.B))
            {
                Debug.Log(
                    $"Key Down Detected on frame {Time.frameCount}. Executing next state update within this frame.");
                m_StateMachine.SetState(StateMachinaUsageExample.State.ButtonDown, true);
            }
        }

        public void StateEnd()
        {
            Debug.Log($"State End: {GetType()}  on frame {Time.frameCount}");
        }
    }

    public class StateButtonDown : StateMachina.IState
    {
        private StateMachina m_StateMachine;
        private int m_UpdateCount = 0;

        public void StateInit<T>(T owner, StateMachina stateMachine) where T : class
        {
            m_StateMachine = stateMachine;
            Debug.Log($"State Init: {GetType()}");
        }

        public void StateStart()
        {
            m_UpdateCount = 0;
            Debug.Log($"State Start: {GetType()} on frame {Time.frameCount}");
        }

        public void StateUpdate()
        {
            m_UpdateCount++;
            if (m_UpdateCount == 1)
            {
                Debug.Log($"First State Update: on frame {Time.frameCount}");
            }

            // If button down detected switch states.
            if (!Input.GetKey(KeyCode.B))
            {
                Debug.Log($"Key Up Detected on frame {Time.frameCount}.");
                m_StateMachine.SetState(StateMachinaUsageExample.State.Idle);
            }
        }

        public void StateEnd()
        {
            Debug.Log($"StateEnd: {GetType()} on frame {Time.frameCount}");
        }
    }
}