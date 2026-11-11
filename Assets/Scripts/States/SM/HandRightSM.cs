namespace States
{
    public class HandRightSM : SM
    {
        public override State DefaultState => new RHandEmpty();
    }
}