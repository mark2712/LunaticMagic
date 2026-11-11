namespace States
{
    public abstract class LHandBase : State
    {
        public override SM SM => SMController.HandLeftSM;
    }
}