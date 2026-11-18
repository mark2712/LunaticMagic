using System;
using UniRx;
using UnityEngine;

// Загрузчик игры: инкапсулирует барьер синхронизации, эмуляцию/стриминг сущностей
// и перевод сессии в активный геймплей.
public class GameLoader : IDisposable
{
    public GameLoadingProgress LoadingProgress { get; } = new();

    private NewGame.INewGameSessionChooser _newGame;
    private GameSession _session;
    private readonly CompositeDisposable _disposables = new();
    private IDisposable _timerSubscription;

    public GameLoader() { }

    public GameLoader(NewGame.INewGameSessionChooser newGame, GameSession session = null)
    {
        _newGame = newGame;
        _session = session ?? (GlobalGame.Session as GameSession);
    }

    public void Start(NewGame.INewGameSessionChooser newGame = null, GameSession session = null)
    {
        if (newGame != null) _newGame = newGame;
        if (session != null) _session = session;
        _session ??= GlobalGame.Session as GameSession;

        if (_session == null)
        {
            Debug.LogError("[GameLoader] Сессия не найдена!");
            return;
        }

        _timerSubscription?.Dispose();

        // 1. Включаем стейт синхронизации сущностей
        _session.IsEntitiesSync.Value = true;
        LoadingProgress.Reset();
        // Debug.Log("[GameLoader] Загрузка началась");

        // 2. Барьер синхронизации через UniRx (игнорирует Time.timeScale = 0 на паузе)
        _timerSubscription = Observable.Interval(TimeSpan.FromMilliseconds(20), Scheduler.MainThreadIgnoreTimeScale)
            .Take(100)
            .Subscribe(
                i =>
                {
                    float progress = i + 1; // 1..100
                    LoadingProgress.SetProgress(progress);
                },
                () =>
                {
                    // 3. Выполняем сценарий старта (поиск сущностей, выдача фокуса)
                    // _newGame?.Execute(_session.EntitiesManager);

                    // 4. Завершение синхронизации и переход в геймплей через Batch
                    _session.IsEntitiesSync.Value = false;
                    _session.IsGameplay.Value = true;
                    _session.IsSessionInitLoading.Value = false;

                    // Debug.Log("[GameLoader] Загрузка и синхронизация успешно завершены. Вход в геймплей.");
                }
            );
    }

    public void Dispose()
    {
        _timerSubscription?.Dispose();
        _disposables.Dispose();
        LoadingProgress.Dispose();
    }
}
