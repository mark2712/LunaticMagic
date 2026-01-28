using ResourceSystem;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

namespace UITK
{
    public sealed class AssetsDebugComponent : UIComponent<Type>
    {
        public AssetsDebugComponent(Type props, string key = "0") : base(props, key) { }
        private IReadOnlyReactiveDictionary<string, ResourceAsset<Dictionary<string, string>>> Assets;
        private readonly ReactiveProperty<DateTime> _update = new(DateTime.UtcNow);

        private VisualTreeAsset _LocalizationAssetDebugUxml;

        public override void Init()
        {
            Assets = Use(Localization.Manager.Assets);
            Use(_update);

            _LocalizationAssetDebugUxml = LoadUITK("Debug/Localization/LocalizationAssetDebug/LocalizationAssetDebug.uxml");

            var refreshBtn = View.Q<Button>("RefreshButton");
            RegisterCallback<ClickEvent>(refreshBtn, _ =>
            {
                _update.Value = DateTime.UtcNow;
            });
        }

        public override void Render()
        {
            var container = View.Q("AssetList");
            foreach (var pair in Assets)
            {
                Node<AssetDebugComponent, string>(pair.Key, _LocalizationAssetDebugUxml, container, pair.Key);
            }
        }
    }
}