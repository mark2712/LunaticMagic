using Game.Session.Sound;
using LocalTime;
using UnityEngine;

namespace SessionSM
{
    // Начальная загрузка сессии (распаковка сейва, первичная инициализация мира)
    public class SessionInitialLoadingState : SessionStateBase
    {
        public override string Name => "InitialLoading";
        public override bool IsGameplay => false;
        public override CursorStateMode CursorMode => CursorStateMode.Visible;
        public override SessionSoundSlot ActiveSoundSlot => SessionSoundSlot.Loading;
        public override bool ResetMusicOnEnter => true;
        public override ITimeState TimeState => TimeStateFactory.Pause();

        public SessionInitialLoadingState(SessionStateManager sm = null) : base(sm)
        {
            RegisterEvent(SessionEvent.InitialLoadingFinished, Computed);
            RegisterEvent(SessionEvent.StartEntitiesSync, Computed);
            RegisterEvent(SessionEvent.FinishEntitiesSync, Computed);
        }
    }
}
