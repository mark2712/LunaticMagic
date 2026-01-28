using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UITK
{
    public sealed class ProfilesListComponent : UIComponent<Type>
    {
        public ProfilesListComponent(Type props, string key = "0") : base(props, key) { }
        private VisualTreeAsset _profileItemUxml;

        public override void Init()
        {
            _profileItemUxml = LoadUITK("Menu/ProfilesAndSaves/Profile/Item/index.uxml");
        }

        public override void Render()
        {
            var profiles = Use(GlobalGame.Profiles.Profiles);
            var container = View.Q("ProfilesList");

            foreach (var kv in profiles)
            {
                var profile = kv.Value;
                Node<ProfileItemComponent, GameProfile>(profile, _profileItemUxml, container, profile.ProfileId.Value);
            }
        }
    }
}