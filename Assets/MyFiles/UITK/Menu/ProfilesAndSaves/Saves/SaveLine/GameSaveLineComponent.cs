using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UITK
{
    public sealed class GameSaveLineComponent : UIComponent<GameSave>
    {
        public GameSaveLineComponent(GameSave props, string key) : base(props, key) { }

        private VisualTreeAsset _loadButton;

        private Label _id;
        private Label _createdAt;
        private Label _comment;
        private Label _type;

        public override void Init()
        {
            _loadButton = LoadUITK("Menu/ProfilesAndSaves/Saves/LoadButton/index.uxml");

            _id = View.Q<Label>("SaveId");
            _createdAt = View.Q<Label>("CreatedAt");
            _comment = View.Q<Label>("Comment");
            _type = View.Q<Label>("SaveType");

            // реактивность
            Use(Props.SaveId);
            Use(Props.CreatedAt);
            Use(Props.Comment);
            Use(Props.SaveType);
        }

        public override void Render()
        {
            _id.text = Props.SaveId.Value;
            _createdAt.text = Props.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss");
            _comment.text = Props.Comment.Value;
            _type.text = Props.SaveType.Value.ToString();

            Node<LoadButtonComponent, GameSave>(Props, _loadButton, View.Q("LoadButtonContainer"));
        }
    }
}