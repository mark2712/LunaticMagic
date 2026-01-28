using System;
using UnityEngine.UIElements;

namespace UITK
{
    public sealed class ProfileSubMenuComponent : UIComponent<GameProfile>
    {
        public ProfileSubMenuComponent(GameProfile props, string key) : base(props, key) { }

        private VisualTreeAsset _saveItem;

        public override void Init()
        {
            _saveItem = LoadUITK("Menu/ProfilesAndSaves/Saves/LoadButton/index.uxml");
        }

        public override void Render()
        {
            var saves = Use(GlobalGame.Profiles.GameProfilesSaves[Props.ProfileId.Value].Saves);
            var list = View.Q("SavesList");

            foreach (var kv in saves)
            {
                Node<LoadButtonComponent, Type>(null, _saveItem, list, kv.Key);
            }
        }
    }
}