namespace States
{
    public abstract class MainBase : State
    {
        public override SM SM => SMController.MainSM;
    }
}