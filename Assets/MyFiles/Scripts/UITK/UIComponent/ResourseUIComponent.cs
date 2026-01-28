using ResourceSystem;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine.UIElements;

namespace UITK
{
    public abstract partial class UIComponent : IDisposable
    {
        protected IResourceController<VisualTreeAsset> ResourceVisualTreeAsset { get; private set; } = ResourceManager.AddressableVisualTreeAsset;
        private readonly List<IResourceBinding<VisualTreeAsset>> ResourceVisualTreeAssetBindings = new();

        protected VisualTreeAsset LoadUITK(string key)
        {
            // var binding = ResourcesVisualTreeAsset.Bind($"Prefabs/UITK/{key}");
            var binding = ResourceVisualTreeAsset.Bind($"Assets/MyFiles/UITK/{key}");
            ResourceVisualTreeAssetBindings.Add(binding);
            return binding.Resource;
        }

        protected void ResourcesVisualTreeAssetDispose()
        {
            foreach (var b in ResourceVisualTreeAssetBindings)
                b.Dispose();

            ResourceVisualTreeAssetBindings.Clear();
        }


        // protected IResourceManager<StyleSheet> ResourceStyleSheet { get; private set; } = ResourceManager.AddressableStyleSheet;
        // private readonly List<IResourceBinding<StyleSheet>> ResourceStyleSheetBindings = new();

        // protected StyleSheet LoadStyle(string key)
        // {
        //     var binding = ResourceStyleSheet.Bind($"Assets/MyFiles/UITK/{key}");
        //     ResourceStyleSheetBindings.Add(binding);
        //     return binding.Resource;
        // }

        // protected void ResourcesStyleSheetDispose()
        // {
        //     foreach (var b in ResourceStyleSheetBindings)
        //         b.Dispose();

        //     ResourceStyleSheetBindings.Clear();
        // }
    }
}