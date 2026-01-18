namespace States
{
    public class FlagsEvents
    {
        public FlagsEvents(SM mainSM)
        {
            Flags.Subscribe(FlagName.Move, _ => mainSM.TriggerEvent(StateEvent.MoveChanged));
            Flags.Subscribe(FlagName.Shift, _ => mainSM.TriggerEvent(StateEvent.ShiftChanged));
            Flags.Subscribe(FlagName.Sneak, _ => mainSM.TriggerEvent(StateEvent.SneakChanged));
            Flags.Subscribe(FlagName.Ground, _ => mainSM.TriggerEvent(StateEvent.GroundChanged));

            Flags.Subscribe(FlagName.LegsRope, _ => mainSM.TriggerEvent(StateEvent.LegsRopeChanged));
            Flags.Subscribe(FlagName.HandsRope, _ => mainSM.TriggerEvent(StateEvent.HandsRopeChanged));
        }
    }
}