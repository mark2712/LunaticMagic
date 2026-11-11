namespace States
{
    public abstract class ModalBase : State
    {
        public override SM SM => SMController.ModalSM;
    }
}