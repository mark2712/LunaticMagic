namespace States
{
    public abstract class RHandBase : State
    {
        public override SM SM => SMController.HandRightSM;
    }
}