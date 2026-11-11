using System.Collections.Generic;
using Entities_1;

namespace Dialog
{
    public static class DialogueData
    {
        public static string Language { get; set; } = "RU";
        public static BaseEntity Player { get; set; } = GameContext.PlayerEntity;
        public static bool DebugMod { get; set; } = true;
    }

    public class DialogueManager
    {
        private Dictionary<string, Dialogue> _dialogues = new();
        public string Language { get; set; } = DialogueData.Language;
        public static BaseEntity Player { get; set; } = GameContext.PlayerEntity;

        public DialogueManager() { }

        public Dialogue CreateDialogueNPC(BaseEntity NPC)
        {
            Answer[] Answers = {
                new Answer("Как пройти в библиотеку?").AddReply(
                    new Reply("Прямо и налево", NPC).Next("Спасибо")
                ),
                new Answer("Новости").AddText("Что нового?").AddReply(
                    new Reply("Всё по старому", NPC).Next("Спасибо")
                ),
                new Answer("Умри").End().Do((answer) => { NPC.NowStats.Health.ChangeNowValue(-1000, true); }),
                new Answer("Выход").AddText("Ничего").Next("Пока")
            };

            Dialogue dialogue = new Dialogue("Разговор с НПС").AddReply(
                new Reply("Привет", NPC).Start(),
                new Reply("Привет, есть минутка?", Player),
                new Reply("А что ты хочешь узнать?", NPC).AddAnswer(Answers)
            ).AddReply(
                new Reply("Спасибо", Player),
                new Reply("Что ещё ты хочешь узнать?", NPC).AddAnswer(Answers)
            ).AddReply(
                new Reply("Пока", NPC).End()
            );

            return AddDialogue(dialogue);
        }

        // public void DialogueNPC(BaseEntity NPC)
        // {
        //     Dialogue dialogue = CreateDialogueNPC(NPC);
        //     dialogue.Open(); // открыть окно диалога
        //     dialogue.Start(); // "Привет"
        //     dialogue.Next(); // "Привет, есть минутка?"
        //     dialogue.Next(); // "А что ты хочешь узнать?"
        //     dialogue.Choose(0); // "Как пройти в библиотеку?" \ "Прямо и налево"
        //     dialogue.Next(); // "Спасибо"
        //     dialogue.Next(); // "Что ещё ты хочешь узнать?"
        //     dialogue.Choose(3); // "Выход" \ "Пока"
        //     dialogue.Next(); // конец диалога
        //     dialogue.Close(); // закрыть окно, сработает автоматчески в конце диалога
        // }

        public DialogueManager SetLanguage(string currentLanguage) { Language = currentLanguage; return this; }
        public Dialogue AddDialogue(Dialogue dialogue) { _dialogues[dialogue.Name] = dialogue; return dialogue; }
        public Dialogue GetDialogue(string name) { return _dialogues.TryGetValue(name, out var dialogue) ? dialogue : null; }
    }
}