using System;
using UniRx;
using UnityEngine.UIElements;

namespace UITK
{
    public class ProfileItemProps
    {
        public GameProfile Profile;
        public IGameProfiles Profiles;
    }

    public sealed class ProfileItemComponent : UIComponent<ProfileItemProps>
    {
        public ProfileItemComponent(ProfileItemProps props, string key = "0") : base(props, key) { }
        private readonly ReactiveProperty<bool> _expanded = new(false);
        private VisualTreeAsset _submenu;

        public override void Init()
        {
            _submenu = LoadUITK("Menu/ProfilesAndSaves/Profile/SubMenu/index.uxml");

            var btn = View.Q<Button>("ProfileButton");
            btn.text = Props?.Profile?.Name?.Value ?? string.Empty;

            Use(_expanded);

            RegisterCallback<ClickEvent>(btn, _ =>
            {
                _expanded.Value = !_expanded.Value;
                if (_expanded.Value && Props != null)
                {
                    Props.Profiles?.LoadSaves(Props.Profile.ProfileId.Value);
                }
            });
        }

        public override void Render()
        {
            if (_expanded.Value && Props != null)
            {
                Node<ProfileSubMenuComponent, ProfileItemProps>(Props, _submenu, View.Q("SubMenuContainer"), "submenu");
            }
        }
    }
}
