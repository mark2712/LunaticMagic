using UniRx;
using UnityEngine;

public sealed class GameTimeController
{
    // ───────── Sources ─────────

    private bool _pauseRequested;
    private bool _consoleRequested;
    private bool _consoleStopsGame;

    private float _gameTimeScale = 1f;

    // ───────── Reactive outputs ─────────

    private readonly ReactiveProperty<bool> _isPaused = new(false);
    private readonly ReactiveProperty<float> _effectiveTimeScale = new(1f);

    public IReadOnlyReactiveProperty<bool> IsPaused => _isPaused;
    public IReadOnlyReactiveProperty<float> EffectiveTimeScale => _effectiveTimeScale;

    public void SetPaused(bool paused)
    {
        if (_pauseRequested == paused) return;
        _pauseRequested = paused;
        Recalculate();
    }

    public void SetConsole(bool active, bool stopGame)
    {
        _consoleRequested = active;
        _consoleStopsGame = stopGame;
        Recalculate();
    }

    public void SetGameTimeScale(float scale)
    {
        _gameTimeScale = Mathf.Max(0f, scale);
        Recalculate();
    }

    private void Recalculate()
    {
        bool paused =
            (_consoleRequested && _consoleStopsGame) ||
            _pauseRequested;

        float timeScale = paused
            ? 0f
            : _gameTimeScale;

        Apply(paused, timeScale);
    }

    private void Apply(bool paused, float timeScale)
    {
        if (_isPaused.Value != paused)
        {
            _isPaused.Value = paused;
            AudioListener.pause = paused;
        }

        if (!Mathf.Approximately(Time.timeScale, timeScale))
        {
            Time.timeScale = timeScale;
            _effectiveTimeScale.Value = timeScale;
        }
    }
}