using UnityEngine;
using EFES.StateMachina;

namespace EFES.StateMachina
{
    public class MonoStateMachinaExample : MonoBehaviour
    {
        readonly StateMachina m_StateMachine = new();

        internal enum State
        {
            Idle,
            ButtonDown,
        }

        void Start()
        {
            m_StateMachine.EnableDebug = true;

            // Assign States
            m_StateMachine.AssignState(new StateIdle(this), State.Idle);
            m_StateMachine.AssignState(new StateButtonDown(this), State.ButtonDown);

            // Start state machine
            m_StateMachine.SetState(State.Idle);
            m_StateMachine.StartStateMachine();
        }

        void Update()
        {
            m_StateMachine.Update();
        }
    }
    
    class StateIdle : StateMachina.State
    {
        private readonly MonoStateMachinaExample m_Owner;

        public StateIdle(MonoStateMachinaExample owner)
        {
            m_Owner = owner;
        }
        
        public override void Init()
        {
            Debug.Log($"{GetType()} initialized with owner: {m_Owner.GetType()}");
        }
        public override void Start()
        {
            Debug.Log($"Entering {GetType()}");
        }   
        
        public override void Update()
        {
            // If button down detected switch states.
            if (Input.GetKey(KeyCode.B))
            {
                Debug.Log("Button Down Detected");
                StateMachine.SetState(MonoStateMachinaExample.State.ButtonDown);
            }
        }
        
        public override void End()
        {
            Debug.Log($"Exiting {GetType()}");
        }
    }
    
    class StateButtonDown : StateMachina.State
    {
        private readonly MonoStateMachinaExample m_Owner;

        public StateButtonDown(MonoStateMachinaExample owner)
        {
            m_Owner = owner;
        }
        
        public override void Init()
        {
            Debug.Log($"{GetType()} initialized with owner: {m_Owner.GetType()}");
        }
        public override void Start()
        {
            Debug.Log($"Entering {GetType()}");
        }   
        
        public override void Update()
        {
            // If button down detected switch states.
            if (!Input.GetKey(KeyCode.B))
            {
                Debug.Log("Button Up Detected");
                StateMachine.SetState(MonoStateMachinaExample.State.Idle);
            }
        }
        
        public override void End()
        {
            Debug.Log($"Exiting {GetType()}");
        }
    }
}