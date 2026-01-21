using ResourceSystem;
using System;
using System.Collections.Generic;
using UniRx;

namespace UITK
{
    /* Для получения локализованного текста */
    public abstract partial class UIComponent : IDisposable
    {
        protected ResourceSystem.ILocalization Localization = GlobalGame.Localization;
        private IDisposable _languageSub;
        private readonly Dictionary<string, IResourceBinding<string>> _locBindings = new();

        protected string Text(string resourceKey, string key)
        {
            var bindingKey = $"{resourceKey}|{key}";

            if (!_locBindings.TryGetValue(bindingKey, out var binding))
            {
                // Создаем нашу прослойку, передавая туда сервис и настройки языков
                binding = new ResourceSystem.TextTwoLangLocalizationBinding(
                    Localization.Text,
                    Localization.Language.Value,
                    ILocalization.BaseLanguage,
                    resourceKey,
                    key
                );
                _locBindings[bindingKey] = binding;
            }

            return binding.Resource;
        }

        protected void SubscribeLanguage()
        {
            if (_languageSub != null) return;
            _languageSub = Localization.Language.Subscribe(_ =>
            {
                LocalizationBindingsDispose();
                ScheduleRender();
            });
        }

        protected void LocalizationBindingsDispose()
        {
            // Вызов Dispose здесь корректно очистит оба внутренних байндинга в прослойке
            foreach (var b in _locBindings.Values)
                b.Dispose();

            _locBindings.Clear();
        }

        protected void LocalizationDispose()
        {
            LocalizationBindingsDispose();
            _languageSub?.Dispose();
        }
    }
}
