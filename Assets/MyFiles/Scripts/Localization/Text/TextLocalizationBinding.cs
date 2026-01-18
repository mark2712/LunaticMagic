public sealed class TextLocalizationBinding : ILocalizationBinding
{
    private readonly ITextLocalization _service;
    private readonly string _key; // == modulePath
    private bool _disposed;

    public TextLocalizationBinding(ITextLocalization service, string key)
    {
        _service = service;
        _key = key;
    }

    public string Text => _service.Resolve(_key);

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _service.Release(_key);
    }
}
