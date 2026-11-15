using System;

namespace Game.Session.Sound
{
    // Контракт аудиоконтроллера сессии (глобальный эмбиент / музыка с 3 слотами).
    public interface ISessionSoundController : IDisposable
    {
        // Текущие треки в 3 слотах буфера
        SessionTrackData CurrentGameplayTrack { get; }
        SessionTrackData CurrentPauseTrack { get; }
        SessionTrackData CurrentLoadingTrack { get; }

        // Установка музыки для слотов (имя трека / путь к ассету)
        void SetGameplayTrack(string trackName, float playbackTime = 0f);
        void SetPauseTrack(string trackName, float playbackTime = 0f);
        void SetLoadingTrack(string trackName, float playbackTime = 0f);

        // Сохранение и загрузка состояния треков
        void Load(SessionSoundData data);
        SessionSoundData Save();
    }
}
