using System.Collections.Generic;
using UnityEngine;

namespace States
{
    public class TakeWeapon : BaseHands
    {
        public override List<ConflictRule> Conflicts => new()
        {
            new ConflictAll<Inventory>(),
        };

        public TakeWeapon() : base()
        {
            RegisterEvent(StateEvent.Mouse1Performed, (state, i) => { return new Hit(); });
            RegisterEvent(StateEvent.Mouse2Performed, (state, i) => { return new HandsFree(); });
        }

        public override void Enter()
        {
            base.Enter();
            GameContext.PlayerAnimationController.SwordRightHand();
        }
    }
}