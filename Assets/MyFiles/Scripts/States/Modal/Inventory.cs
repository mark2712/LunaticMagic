namespace States
{
    public class Inventory : ModalBase
    {
        public Inventory() : base()
        {
            RegisterEvent(StateEvent.TabPerformed, (state, i) => { return new NoneModal(); });
        }

        public override void Enter()
        {
            base.Enter();
            // GameContext.UIManager.ToggleInventory(true);
        }

        public override void Exit()
        {
            base.Exit();
            // GameContext.UIManager.ToggleInventory(false);
        }
    }
}