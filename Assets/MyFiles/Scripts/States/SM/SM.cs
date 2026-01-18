using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace States
{
    public abstract partial class SM
    {
        public SMController smController;
        public virtual State State { get; set; }
        public virtual State DefaultState { get; set; }

        public virtual void UpdateState()
        {
            if (NextLayerState != null)
            {
                GoToLayer();
            }
        }

        public virtual void GoToStateEnter(State newState)
        {
            State = newState;

            foreach (var conflict in newState.Conflicts)
            {
                conflict.Resolve();
            }

            string debugStateColor = this.GetType().Name switch
            {
                "MainSM" => "#ffe2ae",
                "HandsSM" => "#c2aeff",
                "HandRightSM" => "#aec7ff",
                "HandLeftSM" => "#aeecff",
                "ModalSM" => "#edfb7c",
                _ => "#ffffff",
            };
            // Debug.Log($"<color={debugStateColor}>Переход в состояние: <b>{State.GetType().Name}</b> ({this.GetType().Name})</color>");
            State.Enter();
        }

        /*
        ВАЖНО! Переход к сотояниям нужно осуществлять строго через возврат состояния (через очередь событий). Если переход произойдет не как вызов события у текущего сотояния, то могут быть потенциальные проблемы. Используйте такой переход с умом. Так же можно положить новое состояние сразу в State, но это можно делать только в особых случаях.
        */
        public virtual void GoToState(State newState)
        {
            if (!newState.Reentry && State.GetType() == newState.GetType()) { return; }
            State?.Exit();
            GoToStateEnter(newState);
        }

        // Layers
        // используется только для паузы, может терять промежуточные состояния и переходы (НЕБЕЗОПАСНО!)
        private Stack<State> _stateStack = new Stack<State>();
        private State NextLayerState;

        protected void GoToLayer()
        {
            _stateStack.Push(State);
            GoToStateEnter(NextLayerState);
            NextLayerState = null;
        }

        public void GoToLayer(State newState)
        {
            NextLayerState = newState;
        }

        public void ReturnToLayer()
        {
            if (_stateStack.Count > 0)
            {
                State.Exit();
                State = _stateStack.Pop();
                // Debug.Log($"Возврат к состоянию: {State.GetType().Name}");
            }
        }

        // Events
        public void FixedUpdate() { State.FixedUpdate(); }
        public void Update() { State.Update(); }
        public void LateUpdate() { State.LateUpdate(); }

        public void TriggerEvent(StateEvent eventType)
        {
            EventQueue.AddEvent(() => State.InvokeEvent(eventType));
        }

        public State InvokeEvent(StateEvent eventType)
        {
            return State.InvokeEvent(eventType);
        }

        public void ScrollPerformed(InputAction.CallbackContext ctx) { EventQueue.AddEvent(() => State.ScrollPerformed(ctx)); }
    }
}