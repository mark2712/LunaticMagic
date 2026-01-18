using UnityEngine;

namespace States
{
    public abstract class BaseHands : State
    {
        public override SM SM => SMController.HandsSM;

        public BaseHands() : base()
        {
            RegisterEvent(StateEvent.HandsRopeChanged, (state, i) => { return SM.GetHandsState(); });

            RegisterEvent(StateEvent.Mouse1Performed, (state, i) =>
            {
                State legsNewState = SMController.LegsSM.InvokeEvent(StateEvent.Mouse1Performed);
                if (legsNewState == null)
                {
                    return new PunchR(); // у ног нет возможности принять участья в атаке, значит атака только рукой
                }
                else
                {
                    return new Punch(); // если у ног есть возможность атаковать и у рук тоже то атакуем всем телом
                }
            });

            RegisterEvent(StateEvent.F1, (state, i) =>
            {
                Flags.Inverse(FlagName.HandsRope);
                // Flags.Inverse автоматически запустит событие смены состояния на HandsRope, но тут можно сразу вернуть GetHandsState, но будет 2 события перехода в HandsRope.
                // return SM.GetHandsState();
                return null; // рекомендуется оставить этот вариант
            });
        }
    }
}