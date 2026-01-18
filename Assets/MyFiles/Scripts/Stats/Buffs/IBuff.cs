using UnityEngine;

namespace Stats
{
    interface IBuff
    {
        void Enter();
        void Exit();
        void Begin();
        void End();
        void FixedUpdate();
    }

    interface IIntervalBuff : IBuff
    {
        public float Interval { set; get; }
        void Tick();
    }

    interface ITempBuff : IIntervalBuff
    {
        public float Duration { set; get; }
    }
}