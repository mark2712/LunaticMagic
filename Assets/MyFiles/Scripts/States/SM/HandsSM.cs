namespace States
{
    public class HandsSM : SM
    {
        public override State DefaultState => new HandsFree();
        // public HandsSM(SM parentSM) : base(parentSM) { }
    }
}