/// <summary>
/// Разделит ключ на 2 ключа для текущего и базового языка
/// </summary>
public sealed class TextTwoLangLocalizationBinding : ILocalizationBinding
{
    private readonly ITextLocalization _service;
    private readonly string _logicalKey;
    private readonly string _currentLang;
    private readonly string _baseLang;

    private ILocalizationBinding _primary;
    private ILocalizationBinding _fallback;
    private bool _disposed;

    public TextTwoLangLocalizationBinding(ITextLocalization service, string currentLang, string baseLang, string logicalKey)
    {
        _service = service;
        _logicalKey = logicalKey;
        _currentLang = currentLang;
        _baseLang = baseLang;

        // Сразу создаем байндинг для текущего выбранного языка
        _primary = _service.Bind($"{_currentLang}/{_logicalKey}");
    }

    public string Text
    {
        get
        {
            // 1. Пробуем основной язык
            var value = _primary.Text;
            if (value != null) return value;

            // 2. Если мы уже в базовом языке, то искать больше негде
            if (_currentLang == _baseLang) return $"!{_logicalKey}!";

            // 3. Лениво создаем байндинг для базового языка, если его еще нет
            _fallback ??= _service.Bind($"{_baseLang}/{_logicalKey}");

            // 4. Пробуем взять из базового
            value = _fallback.Text;
            if (value != null)
                return value;

            // 5. Совсем ничего не нашли
            return $"!{_logicalKey}!";
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
