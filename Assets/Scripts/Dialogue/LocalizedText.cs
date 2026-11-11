using System;
using System.Collections.Generic;
using Entities_1;

namespace Dialog
{
    public class LocalizedText
    {
        private Dictionary<string, string> _texts = new();
        private string _defaultText;

        public LocalizedText(string name)
        {
            _defaultText = name;
        }

        public LocalizedText Add(string lang, string text)
        {
            _texts[lang] = text;
            return this;
        }

        public string Get(string lang)
        {
            return _texts.TryGetValue(lang, out var text) ? text : _defaultText;
        }

        public override string ToString() => _defaultText;
    }

    // public BaseEntity Player { get; set; }
    // public BaseEntity NPC { get; set; } // Собеседник по умолчанию (если НПС несколько то в реплику можно передать другого)
    // public Dialogue(string name, BaseEntity NPC) { CreateDialogue(name); this.NPC = NPC; }

    // Player = DialogueData.Player;


    // public enum LanguageName
    // {
    //     RU,
    //     EN,
    // }

    // public class Dialogue
    // {
    //     public virtual LanguageName Language => LanguageName.RU;
    //     public virtual string Name => "Название диалога"; // уникальное
    //     public virtual BaseEntity Entity { get; set; }

    //     public Dialogue()
    //     {
    //         Dialogue dialogue = new("Название диалога").SetLanguage(LanguageName.RU).AddReply(
    //             new Reply("название реплики в данном диалоге").AddText("Реплика 1").AddAnswer(
    //                 new Answer("название ответа", функция_которая_сработает_при_выборе_этого_ответа).AddText("Ответ 1"),
    //                 new Answer("название ответа 2", "название следующей реплики в диалоге").AddText("Ответ 2"),
    //                 new Answer("название ответа 3", "Название другого диалога", "название реплики в диалоге").AddText("Ответ 3")
    //             ),
    //             new Reply("название реплики2 в данном диалоге").AddText(LanguageName.RU, "Реплика 2").AddAnswer(
    //                 new Answer("название ответа", функция_которая_сработает_при_выборе_этого_ответа).AddText(LanguageName.RU, "Ответ 1"),
    //                 new Answer("название ответа 2", "название следующей реплики в диалоге").AddText(LanguageName.RU, "Ответ 2"),
    //                 new Answer("название ответа 3", "Название другого диалога", "название реплики в диалоге").AddText(LanguageName.RU, "Ответ 3")
    //             ).SetLanguage(LanguageName.RU)
    //         );



    //         Reply reply = new(this, "название реплики в данном диалоге");
    //         reply.NewAnswer(new Answer("название ответа", функция_которая_сработает_при_выборе_этого_ответа).Add(LanguageName.RU, "Ответ 1"));
    //         reply.NewAnswer(new Answer("название ответа 2", "название следующей реплики в диалоге").Add(LanguageName.RU, "Ответ 2"));
    //         reply.NewAnswer(new Answer("название ответа 3", "Название другого диалога", "название реплики в диалоге").Add(LanguageName.RU, "Ответ 3"));

    //         Create(
    //             "название реплики в данном диалоге", // человекочитаемое название реплики по которому потом можно будет найти 
    //             new List<Answer>(){
    //                 new("название ответа", функция_которая_сработает_при_выборе_этого_ответа), // функция получает ссылку на текущий класс диалога
    //                 new("название ответа 2", "название следующей реплики в диалоге"),
    //                 new("название ответа 3", "Название другого диалога", "название реплики в диалоге"),
    //             },
    //             Entity // сущность которая говорит эту реплику (не обязательно)
    //         );
    //     }

    //     public virtual List<Reply> Replys => new();

    //     public virtual void Create(string text, List<Answer> answers, BaseEntity entity)
    //     {

    //     }

    //     public virtual void Start()
    //     {
    //         // if (Answers.length > 0) { } else { }
    //     }
    //     public virtual void Next() { }
    // }

    // // public class DialogueEvent
    // // { 

    // // }

    // public abstract class Reply
    // {
    //     public virtual string Text => "";
    //     public virtual List<Answer> Answers => new();
    //     public virtual BaseEntity Entity { get; set; }
    //     public virtual void Next() { }
    //     public virtual void Start() { }
    //     public virtual void Exit() { }

    // }

    // public class Answer
    // {
    //     public virtual string Text => "";
    //     public virtual function OnChoose { get; set; }
    //     public virtual void Next() { }
    //     public Answer(string text, function onChoose)
    //     {
    //         Text = text;
    //         OnChoose = onChoose;
    //     }

    //     public Answer(string text, string Next) //Next - "название реплики в диалоге"
    //     {
    //         Text = text;
    //         OnChoose = onChoose;
    //     }
    // }

    // public class Answer1 : Answer
    // {
    //     public override string Text => "Answer1";
    //     public override List<Answer> Answers => new();
    // }
}