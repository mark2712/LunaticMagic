using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UITK
{
    public class GameSaveLineProps
    {
        public GameSave Save;
        public GameProfile Profile;
    }

    public sealed class GameSaveLineComponent : UIComponent<GameSaveLineProps>
    {
        public GameSaveLineComponent(GameSaveLineProps props, string key) : base(props, key) { }

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

            if (Props?.Save != null)
            {
                Use(Props.Save.SaveId);
                Use(Props.Save.CreatedAt);
                Use(Props.Save.Comment);
                Use(Props.Save.SaveType);
            }
        }

        public override void Render()
        {
            if (Props?.Save == null) return;

            _id.text = Props.Save.SaveId.Value;
            _createdAt.text = Props.Save.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss");
            _comment.text = Props.Save.Comment.Value;
            _type.text = Props.Save.SaveType.Value.ToString();

            Node<LoadButtonComponent, GameSaveLineProps>(Props, _loadButton, View.Q("LoadButtonContainer"));
        }
    }
}
