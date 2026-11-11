using UnityEngine;

namespace States
{
    public class BaseMove : BaseGame
    {
        // public override State KeyF_performed() { return BeginDialog(); }
        // public override State KeyT_performed() { return BeginDialog(); }

        public State BeginDialog()
        {
            // Dialog.Dialogue dialog = 
            if (GameContext.DialogueTrigger.TryStartDialogue())
            {
                return new DialogState();
            }
            return null;
        }
    }
}