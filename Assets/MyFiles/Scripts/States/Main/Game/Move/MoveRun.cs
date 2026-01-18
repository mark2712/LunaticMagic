namespace States
{
    public class MoveRun : BaseMove
    {
        public override void Enter()
        {
            base.Enter();
            GameContext.PlayerAnimationController.Run();
        }
    }
}