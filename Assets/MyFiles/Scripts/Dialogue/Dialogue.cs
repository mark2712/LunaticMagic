using System.Collections.Generic;
using Entities_1;

namespace Dialog
{
    public class Dialogue
    {
        public string Name { get; private set; }
        public Dictionary<string, Reply> Replies { get; } = new();
        public List<Reply> RepliesList { get; } = new();
        public Reply FirstReply { get; set; } = null;
        public string Language { get; set; }

        public Dialogue(string name)
        {
            Name = name;
            Language = DialogueData.Language;
        }
        
        public Dialogue SetLanguage(string lang) { Language = lang; return this; }

        public Dialogue AddReply(params Reply[] replies)
        {
            if (RepliesList.Count == 0 && FirstReply == null && replies[0] != null)
            {
                FirstReply = replies[0];
            }

            for (int i = 0; i < replies.Length; i++)
            {
                var reply = replies[i];
                Replies[reply.Name] = reply;
                RepliesList.Add(reply);
                reply.Dialogue = this;
                reply.Language ??= Language;

                if (i > 0) // автоматический Next у предыдущей реплики на эту
                {
                    var prev = replies[i - 1];
                    if (string.IsNullOrEmpty(prev.NextReplyName) && !prev.EndDialogue)
                    {
                        prev.NextReplyName = reply.Name;
                    }
                }
            }
            return this;
        }
        public Reply GetReply(string name) { return Replies.TryGetValue(name, out var reply) ? reply : null; }
    }
}