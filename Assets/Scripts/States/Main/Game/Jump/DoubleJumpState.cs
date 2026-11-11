// namespace States
// {
//     public class DoubleJump : Jump
//     {
//         public override void Enter()
//         {
//             base.Enter();
//             Jump();
//         }

//         protected override void Jump()
//         {
//             StartTimer(200);
//             GameContext.playerController.Jump();
//             if (GameContext.playerController.JumpCount < 2)
//             {
//                 GameContext.playerController.JumpCount = 1;
//             }
//         }
//     }
// }