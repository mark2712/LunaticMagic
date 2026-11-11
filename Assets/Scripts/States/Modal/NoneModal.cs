namespace States
{
    public class NoneModal : ModalBase
    {
        public NoneModal() : base()
        {
            RegisterEvent(StateEvent.TabPerformed, (state, i) => { return new Inventory(); });

        }
    }
}