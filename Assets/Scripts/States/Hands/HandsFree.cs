using UnityEngine;

namespace States
{
    public class HandsFree : BaseHands
    {
        public HandsFree() : base()
        {
            RegisterEvent(StateEvent.Mouse2Performed, (state, i) => { return new TakeWeapon(); });
            RegisterEvent(StateEvent.TabPerformed, (state, i) => { return SMController.ModalSM.InvokeEvent(StateEvent.TabPerformed); });
        }

        public override void Enter()
        {
            base.Enter();
            // GameContext.playerAnimationController.SwordHandLeft();
            GameContext.PlayerAnimationController.HandsNone();
        }
    }
}