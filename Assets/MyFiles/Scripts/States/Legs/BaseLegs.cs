using UnityEngine;

namespace States
{
    public abstract class BaseLegs : State
    {
        public override SM SM => SMController.LegsSM;

        public BaseLegs() : base()
        {
            RegisterEvent(StateEvent.LegsRopeChanged, (state, i) => { return SM.GetLegsState(); });

            RegisterEvent(StateEvent.F2, (state, i) =>
            {
                Flags.Inverse(FlagName.LegsRope);
                return null;
            });
        }
    }
}