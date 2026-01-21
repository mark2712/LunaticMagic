namespace ResourceSystem
{
    /// <summary>
    /// Разделит ключ на 2 ключа для текущего и базового языка
    /// </summary>
    public sealed class TextTwoLangLocalizationBinding : IResourceBinding<string>
    {
        public string Key { get; private set; }
        public string DictKey { get; private set; }
        public string ResourceKey { get; private set; }

        private readonly TextLocalizationService _service;

        private readonly string _currentLang;
        private readonly string _baseLang;

        private IResourceBinding<string> _primary;
        private IResourceBinding<string> _fallback;
        private bool _disposed;

        public TextTwoLangLocalizationBinding(TextLocalizationService service, string currentLang, string baseLang, string resourceKey, string dictKey)
        {
            _service = service;
            ResourceKey = resourceKey;
            DictKey = dictKey;
            Key = $"{ResourceKey}|{DictKey}";
            _currentLang = currentLang;
            _baseLang = baseLang;

            // Сразу создаем байндинг для текущего выбранного языка
            _primary = _service.Bind($"{_currentLang}/{ResourceKey}", DictKey);
        }

        public string Resource
        {
            get
            {
                // 1. Пробуем основной язык
                var value = _primary.Resource;
                if (value != null) return value;

                // 2. Если мы уже в базовом языке, то искать больше негде
                if (_currentLang == _baseLang) return $"!{Key}!";

                // 3. Лениво создаем байндинг для базового языка, если его еще нет
                _fallback ??= _service.Bind($"{_baseLang}/{ResourceKey}", DictKey);

                // 4. Пробуем взять из базового
                value = _fallback.Resource;
                if (value != null)
                    return value;

                // 5. Совсем ничего не нашли
                return $"!{Key}!";
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _primary?.Dispose();
            _fallback?.Dispose();
        }
    }
}