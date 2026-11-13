using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UITK
{
    public sealed class ProfilesListComponent : UIComponent<IGameProfiles>
    {
        public ProfilesListComponent(IGameProfiles props, string key = "0") : base(props, key) { }
        private VisualTreeAsset _profileItemUxml;

        public override void Init()
        {
            _profileItemUxml = LoadUITK("Menu/ProfilesAndSaves/Profile/Item/index.uxml");
        }

        public override void Render()
        {
            if (Props == null) return;

            var profiles = Use(Props.Profiles);
            var container = View.Q("ProfilesList");

            foreach (var kv in profiles)
            {
                var profile = kv.Value;
                Node<ProfileItemComponent, ProfileItemProps>(
                    new ProfileItemProps { Profile = profile, Profiles = Props },
                    _profileItemUxml,
                    container,
                    profile.ProfileId.Value
                );
            }
        }
    }
}
