namespace States
{
    public class LegsSM : SM
    {
        public override State DefaultState => new LegsFree();
    }
}