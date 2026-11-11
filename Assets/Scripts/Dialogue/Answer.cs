using System;

namespace Dialog
{
    public class Answer
    {
        public string Name { get; }
        public Reply Reply { get; set; }
        public string Language { get; set; }
        public string Text { get; private set; }
        public string NextReplyName { get; set; }
        public Action<Answer> Action { get; set; }
        public bool EndDialogue { get; set; } = false;

        public Answer(string name, Action<Answer> action = null)
        {
            Name = name;
            AddText(name);
            Do(action);
        }

        public Answer(string name, string nextReplyName)
        {
            Name = name;
            Text = name;
            NextReplyName = nextReplyName;
        }

        public Answer Do(Action<Answer> action) { Action = action; return this; }
        public Answer AddText(string text) { Text = text; return this; }
        public Answer AddText(string lang, string text) { Language = lang; Text = text; return this; }

        public Answer AddReply(params Reply[] replies)
        {
            Reply.AddReply(replies);
            return this;
        }

        public Answer Next(string name) { NextReplyName = name; return this; }
        public Answer End() { EndDialogue = true; return this; }

        // public void Choose()
        // {
        //     // if (NextDialogueName != null && NextDialogueName != "") { }
        //     if (NextReplyName != null && NextReplyName != "")
        //     {

        //     }
        //     Action?.Invoke(this);
        // }
    }
}