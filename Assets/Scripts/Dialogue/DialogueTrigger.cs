using Entities_1;
using UnityEngine;

namespace Dialog
{
    public class DialogueTrigger
    {
        public DialogueTriggerMB dialogueTriggerMB;

        public DialogueTrigger(Transform dialogueTrigger, BaseEntity PlayerEntity)
        {
            dialogueTriggerMB = dialogueTrigger.GetComponent<DialogueTriggerMB>();
            dialogueTriggerMB.Player = PlayerEntity;
        }

        public bool TryStartDialogue()
        {
            var targetNPC = dialogueTriggerMB.GetClosestNPC();
            if (targetNPC == null)
            {
                Debug.Log("Рядом нет сущностей для диалога.");
                return false;
            }

            Debug.Log($"Можно начать диалог с {targetNPC.name}");

            // попробовать начать диалог. Если диалог есть то перейти в состояние диалога. Так же 

            // Здесь можно вызвать:
            // DialogueSession.Start(...), или targetNPC.StartDialogue(), и т.п.

            return true;
        }
    }
}