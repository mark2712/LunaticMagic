using Game.Session.Sound;
using LocalTime;
using UnityEngine;

namespace SessionSM
{
    // Синхронизация сущностей (стриминг чанков, блэкаут при телепорте/переходе)
    public class SessionEntitiesSyncState : SessionStateBase
    {
        public override string Name => "EntitiesSync";
        public override bool IsGameplay => false;
        public override CursorStateMode CursorMode => CursorStateMode.Locked;
        public override SessionSoundSlot ActiveSoundSlot => SessionSoundSlot.Gameplay;
        public override bool ResetMusicOnEnter => false;
        public override ITimeState TimeState => TimeStateFactory.Pause();

        public SessionEntitiesSyncState(SessionStateManager sm = null) : base(sm)
        {
            RegisterEvent(SessionEvent.FinishEntitiesSync, Computed);
        }
    }
}
