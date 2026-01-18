using System.Collections.Generic;
using Entities_1;

namespace Dialog
{
    public class Reply
    {
        public string Name { get; private set; }
        public Dialogue Dialogue { get; set; }
        public Dictionary<string, Answer> Answers { get; } = new();
        public List<Answer> AnswersList { get; } = new();
        public string Language { get; set; }
        public string Text { get; private set; }
        public string NextReplyName { get; set; }
        public float AutoNextTime { get; set; } = -1f;
        public BaseEntity Entity { get; set; }
        public bool EndDialogue { get; set; }  = false;

        public Reply(string name) { CreateReply(name); }
        public Reply(string name, BaseEntity entity) { CreateReply(name); Entity = entity; }
        public void CreateReply(string name)
        {
            Name = name;
            AddText(name);
        }
        public Reply AddText(string text) { Text = text; return this; }
        public Reply AddText(string lang, string text) { Language = lang; Text = text; return this; }
        public Reply AddAnswer(params Answer[] answers)
        {
            foreach (var answer in answers)
            {
                Answers[answer.Name] = answer;
                AnswersList.Add(answer);
                answer.Reply = this;
                answer.Language ??= Language;
            }
            return this;
        }
        public Reply AddReply(params Reply[] replies)
        {
            Dialogue.AddReply(replies);
            return this;
        }
        public Reply Start() { Dialogue.FirstReply = this; return this; }
        public Reply Next(string name) { NextReplyName = name; return this; }
        public Reply End() { EndDialogue = true; return this; }
    }
}