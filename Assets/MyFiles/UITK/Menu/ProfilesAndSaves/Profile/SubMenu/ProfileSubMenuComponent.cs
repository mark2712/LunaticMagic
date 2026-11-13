using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UITK
{
    public sealed class ProfileSubMenuComponent : UIComponent<ProfileItemProps>
    {
        public ProfileSubMenuComponent(ProfileItemProps props, string key) : base(props, key) { }

        private VisualTreeAsset _saveLine;

        public override void Init()
        {
            _saveLine = LoadUITK("Menu/ProfilesAndSaves/Saves/SaveLine/index.uxml");

            var btn = View.Q<Button>("NewGameButton");
            RegisterCallback<ClickEvent>(btn, _ =>
            {
                if (Props?.Profile != null)
                {
                    GlobalGame.NewGameSession(Props.Profile, new NewGame.NewGame());
                }
            });
        }

        public override void Render()
        {
            if (Props?.Profiles == null || Props.Profile == null) return;

            string profileId = Props.Profile.ProfileId.Value;
            if (!Props.Profiles.GameProfilesSaves.ContainsKey(profileId)) return;

            var saves = Use(Props.Profiles.GameProfilesSaves[profileId].Saves);
            var list = View.Q("SavesList");

            foreach (var kv in saves)
            {
                Node<GameSaveLineComponent, GameSaveLineProps>(
                    new GameSaveLineProps { Save = kv.Value, Profile = Props.Profile },
                    _saveLine,
                    list,
                    kv.Key
                );
            }
        }
    }
}
