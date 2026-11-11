using System;

namespace UITK
{
    public abstract partial class UIComponent : IDisposable
    {
        public bool IsActive { get; private set; } = true;

        public void Active()
        {
            if (IsActive) return;
            IsActive = true;
            OnActive();
            ScheduleRender();
        }

        public void Disable()
        {
            if (!IsActive) return;
            IsActive = false;
            OnDisable();
            ScheduleRender();
        }

        public void ChangeActive()
        {
            if (IsActive) { Disable(); } else { Active(); }
        }
    }
}