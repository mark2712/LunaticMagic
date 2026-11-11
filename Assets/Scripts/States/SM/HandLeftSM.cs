namespace States
{
    public class HandLeftSM : SM
    {
        public override State DefaultState => new LHandEmpty();
    }
}