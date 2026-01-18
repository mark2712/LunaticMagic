using UnityEngine;

namespace States
{
    public class Jump : BaseAir
    {
        public override bool Reentry => true;

        public Jump() : base()
        {
            // RegisterEvent(StateEvent.MyEvent, (state, i) =>
            // {
            //     var result = InvokeEvent(StateEvent.MyEvent, i - 1);
            //     Debug.Log("MyEvent in Jump");
            //     return null;
            // });

            RegisterEvent(StateEvent.JumpFinished, (state, i) =>
            {
                return JumpFinished();
            });

            RegisterEvent(StateEvent.SpacePerformed, (state, i) => { return null; });

            RegisterEvent(StateEvent.MoveChanged, (state, i) => { return null; });
            RegisterEvent(StateEvent.GroundChanged, (state, i) => { return null; });
            RegisterEvent(StateEvent.ShiftChanged, (state, i) => { return null; });
            RegisterEvent(StateEvent.SneakChanged, (state, i) => { return null; });
        }


        public override void Enter()
        {
            base.Enter();
            DoJump();
            // RegisterEvent(StateEventType.JumpFinished, _ => JumpFinished());

            InvokeEvent(StateEvent.MyEvent);
        }

        protected virtual void DoJump()
        {
            StartTimer(200);
            GameContext.PlayerController.Jump();
            GameContext.PlayerAnimationController.Jump();
        }

        public override void Update()
        {
            base.Update();

            if (IsTimerFinished())
            {
                SM.TriggerEvent(StateEvent.JumpFinished);
                // JumpFinished();
            }
        }

        public State JumpFinished() { return SM.GetGameState(); }
    }
}