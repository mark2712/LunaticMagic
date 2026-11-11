using System;

namespace Dialog
{
    public class Session
    {
        public Dialogue Dialogue { get; }
        private Reply currentReply;
        public bool isEnded;
        public bool NowOpen = false;

        public Session(Dialogue dialogue)
        {
            Dialogue = dialogue;
        }

        public void Start()
        {
            currentReply = Dialogue.FirstReply;
            ShowCurrentReply();
        }

        public void Next()
        {
            if (currentReply?.EndDialogue == true || string.IsNullOrEmpty(currentReply?.NextReplyName))
            {
                End();
                return;
            }

            if (Dialogue.Replies.TryGetValue(currentReply.NextReplyName, out var next))
            {
                currentReply = next;
                ShowCurrentReply();
            }
            else
            {
                End();
            }
        }

        public void Choose(int index)
        {
            var answer = currentReply.AnswersList?[index];
            if (answer?.Reply != null)
            {
                currentReply = answer.Reply;
                answer.Action?.Invoke(answer);
                ShowCurrentReply();
            }
        }

        public void End()
        {
            isEnded = true;
            // DialogueUI.Close();
        }

        private void ShowCurrentReply()
        {
            // DialogueUI.ShowReply(currentReply);
        }
    }
}