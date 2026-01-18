using System;
using UniRx;
using UnityEngine.UIElements;

namespace UITK
{
    public sealed class ProfileItemComponent : UIComponent<GameProfile>
    {
        public ProfileItemComponent(GameProfile props, string key = "0") : base(props, key) { }
        private readonly ReactiveProperty<bool> _expanded = new(false);
        private VisualTreeAsset _submenu;

        public override void Init()
        {
            _submenu = VisualTreeAssetManager.LoadUITK("ProfilesAndSaves/ProfileSubMenu");

            var btn = View.Q<Button>("ProfileButton");
            btn.text = Props.Name.Value;

            Use(_expanded);

            RegisterCallback<ClickEvent>(btn, _ =>
            {
                _expanded.Value = !_expanded.Value;
                if (_expanded.Value)
                {
                    GlobalGame.Profiles.LoadSaves(Props.ProfileId.Value);
                }
            });
        }

        public override void Render()
        {
            if (_expanded.Value)
            {
                Node<ProfileSubMenuComponent, GameProfile>(Props, _submenu, View.Q("SubMenuContainer"), "submenu");
            }
        }
    }
}