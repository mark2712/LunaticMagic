using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UITK
{
    public sealed class ProfileSubMenuComponent : UIComponent<GameProfile>
    {
        public ProfileSubMenuComponent(GameProfile props, string key) : base(props, key) { }

        private VisualTreeAsset _saveLine;

        public override void Init()
        {
            _saveLine = LoadUITK("Menu/ProfilesAndSaves/Saves/SaveLine/index.uxml");

            var btn = View.Q<Button>("NewGameButton");
            RegisterCallback<ClickEvent>(btn, _ =>
            {
                GlobalGame.ChangeSession(Props);
            });
        }

        public override void Render()
        {
            var saves = Use(UIGlobalState.Profiles.GameProfilesSaves[Props.ProfileId.Value].Saves);
            var list = View.Q("SavesList");

            foreach (var kv in saves)
            {
                Node<GameSaveLineComponent, GameSave>(kv.Value, _saveLine, list, kv.Key);
            }
        }
    }
}