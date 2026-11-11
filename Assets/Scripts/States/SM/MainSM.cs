namespace States
{
    public class MainSM : SM
    {
        public override State DefaultState => GetGameState();
    }
}