using System;
using System.Collections.Generic;
using UniRx;

namespace UITK
{
    /* Для получения локализованного текста */
    public abstract partial class UIComponent : IDisposable
    {
        protected ILocalization Localization = GlobalGame.Localization;
        private IDisposable _languageSub;
        private readonly Dictionary<string, ILocalizationBinding> _locBindings = new(); // Список ILocalizationBinding у этого компонента

        protected string Text(string key)
        {
            if (!_locBindings.TryGetValue(key, out var binding))
            {
                // Создаем нашу прослойку, передавая туда сервис и настройки языков
                binding = new TextTwoLangLocalizationBinding(Localization.Text, Localization.Language.Value, ILocalization.BaseLanguage, key);
                _locBindings[key] = binding;
            }

            return binding.Text;
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
