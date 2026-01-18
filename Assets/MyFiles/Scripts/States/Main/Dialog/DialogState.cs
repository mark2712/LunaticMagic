using UnityEngine;

namespace States
{
    public class DialogState : State
    {
        public override SM SM => SMController.ModalSM;
        // public Dialogue.Dialogue Dialogue;
        // public Dialog(Dialogue.Dialogue dialogue)
        // {
        //     Dialogue = dialogue;
        // }

        public override void Enter()
        {
            // открыть окно диалога
            // сказать нпс что сейчавс мы с ним говорим
            Debug.Log("Диалог открыт");
        }
        public override void Exit()
        {
            // закрыть окно диалога
            // сказать нпс что мы больше не говорим с ним
            Debug.Log("Диалог закрыт");
        }

        // public override State ConsolePerformed()
        // {
        //     return SM.GetGameState();
        // }

        // public override State KeyI_performed()
        // {
        //     Debug.Log("Диалог уже открыт");
        //     return null;
        // }
    }
}