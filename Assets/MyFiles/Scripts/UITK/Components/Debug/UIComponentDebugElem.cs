using System;
using UnityEngine.UIElements;

namespace UITK
{
    public class UIComponentDebugElem : UIComponent<Type>
    {
        // public VisualTreeAsset GameProfilesTemp;

        public UIComponentDebugElem(Type props, string key = "0") : base(props, key) { }

        public override void Init()
        {
            // GameProfilesTemp = VisualTreeAssetManager.LoadUITK("Profiles");
        }

        public override void Render()
        {
            // Node<UIComponentProfiles, Type>(null, GameProfilesTemp, AllContainer, "0");
        }
    }
}