using System;
using UniRx;

namespace UITK
{
    public class UIState
    {
        public readonly ReactiveProperty<bool> IsMainMenuOpen = new(false);

        public void Load(UIStateData data)
        {
            IsMainMenuOpen.Value = data.IsMainMenuOpen;
        }

        public UIStateData Save()
        {
            UIStateData data = new()
            {
                IsMainMenuOpen = IsMainMenuOpen.Value
            };
            return data;
        }
    }

    [Serializable]
    public class UIStateData
    {
        public bool IsMainMenuOpen = false;
    }
}