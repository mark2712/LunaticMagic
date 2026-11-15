using System;

namespace Game.Session.Sound
{
    // Контроллер аудиосистемы сессии. Управляет 3 слотами музыки/эмбиента (Gameplay, Pause, Loading),
    // сохраняет и восстанавливает их состояние и таймкоды при сериализации сессии.
    public class SessionSoundController : ISessionSoundController
    {
        // -----------------------------------------------------------------------------------------
        // Поля состояния буфера треков (3 слота)
        // -----------------------------------------------------------------------------------------
        // НЕ ДОДЕЛАНО:
        // - Нет ссылок на компоненты AudioSource (или инкапсулированный аудио-сервис) для физического вывода звука в Unity.
        // - Нет интеграции с ResourceManager / Addressables для асинхронного биндинга AudioClip.
        // - Нет подписки на SessionPauseController (IsPaused / CurrentMode) для автоматического переключения слотов
        //   (GameplayTrack -> PauseTrack на PauseScreen, удержание GameplayTrack при FocusTransition).
        // - Нет реактивных свойств (UniRx ReactiveProperty / Observable) для уведомления внешних систем (UI, микшер) о смене трека.
        private SessionTrackData _gameplayTrack = new();
        private SessionTrackData _pauseTrack = new();
        private SessionTrackData _loadingTrack = new();

        private bool _isDisposed;

        // -----------------------------------------------------------------------------------------
        // Свойства текущих треков в слотах буфера
        // -----------------------------------------------------------------------------------------

        // Текущий трек геймплея (Слот 1)
        public SessionTrackData CurrentGameplayTrack => _gameplayTrack;

        // Текущий трек паузы (Слот 2: занят всегда, пустая строка = тишина)
        public SessionTrackData CurrentPauseTrack => _pauseTrack;

        // Текущий трек загрузки (Слот 3: занят во время InitialLoading)
        public SessionTrackData CurrentLoadingTrack => _loadingTrack;
        // НЕ ДОДЕЛАНО В СВОЙСТВАХ ТРЕКОВ:
        // - Сейчас свойства возвращают прямую ссылку на внутренний SessionTrackData; для инкапсуляции в будущем
        //   желательно возвращать копию либо readonly-интерфейс/структуру, чтобы внешние классы не могли мутировать поля напрямую.
        // - PlaybackTime в возвращаемом объекте отражает последнее зафиксированное значение, а не текущий audioSource.time в реальном времени.

        // -----------------------------------------------------------------------------------------
        // Установка музыки для слотов
        // -----------------------------------------------------------------------------------------

        public void SetGameplayTrack(string trackName, float playbackTime = 0f)
        {
            _gameplayTrack = new SessionTrackData
            {
                TrackName = trackName ?? string.Empty,
                PlaybackTime = Math.Max(0f, playbackTime)
            };

            // НЕ ДОДЕЛАНО:
            // 1. Загрузка AudioClip через ResourceManager / Addressables по пути/ключу trackName.
            // 2. Логика плавного перехода (Crossfade / Fade In / Fade Out) между предыдущим и новым треком.
            // 3. Проверка текущего состояния сессии: если сейчас активен режим Gameplay (или FocusTransition),
            //    немедленно запустить воспроизведение нового трека с позиции playbackTime.
            // 4. Остановка или выгрузка предыдущего аудиоклипа, если он больше не используется.
        }

        public void SetPauseTrack(string trackName, float playbackTime = 0f)
        {
            _pauseTrack = new SessionTrackData
            {
                TrackName = trackName ?? string.Empty,
                PlaybackTime = Math.Max(0f, playbackTime)
            };

            // НЕ ДОДЕЛАНО:
            // 1. Загрузка AudioClip для экрана паузы через ResourceManager.
            // 2. Если сессия прямо сейчас находится на паузе (PauseMode.PauseScreen), немедленно включить данный трек.
            // 3. Корректная обработка тишины: если trackName пустой, физически глушить звук слота паузы.
        }

        public void SetLoadingTrack(string trackName, float playbackTime = 0f)
        {
            _loadingTrack = new SessionTrackData
            {
                TrackName = trackName ?? string.Empty,
                PlaybackTime = Math.Max(0f, playbackTime)
            };

            // НЕ ДОДЕЛАНО:
            // 1. Загрузка AudioClip загрузочного экрана через ResourceManager.
            // 2. Если сессия находится в процессе начальной загрузки (PauseMode.InitialLoading), запустить этот трек.
        }

        // -----------------------------------------------------------------------------------------
        // Сохранение, загрузка и жизненный цикл (Полностью реализовано)
        // -----------------------------------------------------------------------------------------

        /// <summary>
        /// Загружает состояние аудиоконтроллера сессии из структуры сохранения.
        /// Восстанавливает треки для всех трёх слотов (Gameplay, Pause, Loading).
        /// </summary>
        public void Load(SessionSoundData data)
        {
            if (data == null)
            {
                _gameplayTrack = new SessionTrackData();
                _pauseTrack = new SessionTrackData();
                _loadingTrack = new SessionTrackData();
                return;
            }

            _gameplayTrack = data.GameplayTrack != null
                ? new SessionTrackData
                {
                    TrackName = data.GameplayTrack.TrackName ?? string.Empty,
                    PlaybackTime = Math.Max(0f, data.GameplayTrack.PlaybackTime)
                }
                : new SessionTrackData();

            _pauseTrack = data.PauseTrack != null
                ? new SessionTrackData
                {
                    TrackName = data.PauseTrack.TrackName ?? string.Empty,
                    PlaybackTime = Math.Max(0f, data.PauseTrack.PlaybackTime)
                }
                : new SessionTrackData();

            _loadingTrack = data.LoadingTrack != null
                ? new SessionTrackData
                {
                    TrackName = data.LoadingTrack.TrackName ?? string.Empty,
                    PlaybackTime = Math.Max(0f, data.LoadingTrack.PlaybackTime)
                }
                : new SessionTrackData();

            // Примечание: При подключении AudioSource после восстановления данных треков при необходимости
            // здесь может инициироваться синхронизация playbackTime с активным AudioSource в зависимости от текущего стейта сессии.
        }

        /// <summary>
        /// Формирует снимок данных буфера музыки сессии (все 3 слота) для сохранения на диск.
        /// </summary>
        public SessionSoundData Save()
        {
            // Примечание: При наличии подключенного AudioSource перед возвратом данных здесь можно опрашивать
            // активный AudioSource (например, _audioSource.time) для записи актуального PlaybackTime играющего трека.
            return new SessionSoundData
            {
                GameplayTrack = new SessionTrackData
                {
                    TrackName = _gameplayTrack?.TrackName ?? string.Empty,
                    PlaybackTime = Math.Max(0f, _gameplayTrack?.PlaybackTime ?? 0f)
                },
                PauseTrack = new SessionTrackData
                {
                    TrackName = _pauseTrack?.TrackName ?? string.Empty,
                    PlaybackTime = Math.Max(0f, _pauseTrack?.PlaybackTime ?? 0f)
                },
                LoadingTrack = new SessionTrackData
                {
                    TrackName = _loadingTrack?.TrackName ?? string.Empty,
                    PlaybackTime = Math.Max(0f, _loadingTrack?.PlaybackTime ?? 0f)
                }
            };
        }

        /// <summary>
        /// Освобождает ресурсы аудиоконтроллера, останавливает треки и сбрасывает данные слотов.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            // Освобождение подписок, остановка AudioSource (когда будет подключен) и очистка буфера треков
            _gameplayTrack = new SessionTrackData();
            _pauseTrack = new SessionTrackData();
            _loadingTrack = new SessionTrackData();
        }
    }
}
