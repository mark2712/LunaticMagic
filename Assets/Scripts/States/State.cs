using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace States
{
    public abstract class ConflictRule
    {
        public SMController SMController = GameContext.SMController;
        public abstract void Resolve();
    }

    public abstract class State
    {
        public SMController SMController = GameContext.SMController;
        public virtual SM SM { get; set; }
        public virtual bool Reentry => true; // разрешен переход из текущего состояния в это же состояния ThisState -> ThisState


        /* Конфликтные состояния */
        public virtual List<ConflictRule> Conflicts => new(); // все указанные State перейдут в DefaultState 

        // ConflictAll - если состояние это TNotAllowedState или его потомок то будет принудительный переход в DefaultState
        public class ConflictAll<TNotAllowedState> : ConflictRule where TNotAllowedState : State, new()
        {
            public override void Resolve()
            {
                foreach (var sm in SMController.allSM)
                {
                    if (sm.State is TNotAllowedState || sm.State.GetType().IsSubclassOf(typeof(TNotAllowedState)))
                    {
                        sm.GoToState(sm.DefaultState);
                    }
                }
            }
        }

        // ConflictAll - если состояние это TNotAllowedState или его потомок то будет принудительный переход в TGoToStste
        public class ConflictAll<TNotAllowedState, TGoToStste> : ConflictRule where TNotAllowedState : State where TGoToStste : State, new()
        {
            public override void Resolve()
            {
                foreach (var sm in SMController.allSM)
                {
                    if (sm.State is TNotAllowedState || sm.State.GetType().IsSubclassOf(typeof(TNotAllowedState)))
                    {
                        sm.GoToState(new TGoToStste());
                    }
                }
            }
        }


        /* Таймер */
        private Dictionary<string, float> timers = new Dictionary<string, float>();

        protected void StartTimer(float durationMs, string name = "Timer")
        {
            timers[name] = Time.time + durationMs / 1000f;
        }

        protected bool IsTimerFinished(string name = "Timer")
        {
            if (timers.TryGetValue(name, out float endTime))
            {
                if (Time.time >= endTime)
                {
                    timers[name] = Time.time; // Сброс таймера
                    return true;
                }
            }
            return false;
        }

        /* Переход в состояние (НЕБЕЗОПАСНО! лучше использовать InvokeEvent) */
        // protected virtual void GoToState<TNewState>() where TNewState : State
        // {
        //     TNewState newState = Activator.CreateInstance(typeof(TNewState)) as TNewState;
        //     SM.GoToState(newState);
        // }
        // protected virtual void GoToState(State newState)
        // {
        //     SM.GoToState(newState);
        // }


        /* События */
        public readonly Dictionary<StateEvent, List<Func<State, int, State>>> _events = new();

        /// <summary>
        /// Регистрирует обработчик события для текущего состояния.
        /// </summary>
        protected void RegisterEvent(StateEvent evt, Func<State, int, State> handler)
        {
            if (!_events.TryGetValue(evt, out var handlers))
            {
                handlers = new List<Func<State, int, State>>();
                _events[evt] = handlers;
            }

            handlers.Add(handler);
        }

        /// <summary>
        /// Вызывает обработчик события для текущего состояния или базовых состояний.
        /// </summary>
        public virtual State InvokeEvent(StateEvent evt, int? index = null)
        {
            if (!_events.TryGetValue(evt, out var handlers))
            {
                // DebugLog($"Нет события {evt}");
                return null;
            }

            int currentIndex = index ?? (handlers.Count - 1);

            if (currentIndex < 0 || currentIndex >= handlers.Count)
            {
                DebugLog($"Нет события для номера {currentIndex} | {evt}");
                return null;
            }

            return _events[evt][currentIndex](this, currentIndex);
        }

        /// <summary>
        /// Вызывает обработчик базового состояния для события. (Аналог base.InvokeEvent в наследовании)
        /// </summary>
        protected State InvokeEventBase(StateEvent evt, int currentIndex)
        {
            return InvokeEvent(evt, currentIndex - 1);
        }

        private void DebugLog(object str)
        {
#if UNITY_EDITOR
            Debug.LogWarning(str);
#endif
        }


        /* SM События */
        public virtual void Enter() { }
        public virtual void Exit() { }

        /* События MonoBehaviour */
        public virtual void FixedUpdate() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }


        /* Прочие события */
        public virtual State ScrollPerformed(InputAction.CallbackContext ctx) { return null; }
        public virtual void RegisterEventsShiftSneak()
        {
            RegisterEvent(StateEvent.ShiftPerformed, (state, i) => { Flags.Set(FlagName.Shift, true); return null; });
            RegisterEvent(StateEvent.ShiftCanceled, (state, i) => { Flags.Set(FlagName.Shift, false); return null; });
            RegisterEvent(StateEvent.KeyX, (state, i) => { Flags.Inverse(FlagName.Shift); return null; });

            RegisterEvent(StateEvent.CtrlPerformed, (state, i) => { Flags.Set(FlagName.Sneak, true); return null; });
            RegisterEvent(StateEvent.CtrlCanceled, (state, i) => { Flags.Set(FlagName.Shift, false); return null; });
            RegisterEvent(StateEvent.AltPerformed, (state, i) => { Flags.Set(FlagName.Shift, true); return null; });
            RegisterEvent(StateEvent.AltCanceled, (state, i) => { Flags.Set(FlagName.Shift, false); return null; });
            RegisterEvent(StateEvent.KeyC, (state, i) => { Flags.Inverse(FlagName.Sneak); return null; });
        }
    }
}
