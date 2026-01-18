namespace States
{
    public class ModalSM : SM
    {
        public override State DefaultState => new NoneModal();
    }
}