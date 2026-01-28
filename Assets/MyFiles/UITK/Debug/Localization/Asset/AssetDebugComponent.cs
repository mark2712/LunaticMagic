using ResourceSystem;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine.UIElements;

namespace UITK
{
    public sealed class AssetDebugComponent : UIComponent<string>
    {
        public AssetDebugComponent(string props, string key = "0") : base(props, key) { }

        private Label _key;
        private Label _refCount;
        private Label _lastUse;

        public override void Init()
        {
            _key = View.Q<Label>("Key");
            _refCount = View.Q<Label>("RefCount");
            _lastUse = View.Q<Label>("LastUse");

            Use(Localization.Manager.Assets[Props].LastUseTime);
        }

        public override void Render()
        {
            var asset = Localization.Manager.Assets[Props];
            _key.text = asset.Key;
            _refCount.text = $"RefCount: {asset.RefCount}";
            _lastUse.text = $"LastUse: {asset.LastUseTime.Value:HH:mm:ss}";

        }
    }
}