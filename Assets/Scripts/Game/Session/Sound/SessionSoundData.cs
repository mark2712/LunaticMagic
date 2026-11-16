using System;

namespace Game.Session.Sound
{
    // Данные для сохранения и восстановления состояния треков сессии.
    [Serializable]
    public class SessionSoundData
    {
        public SessionTrackData GameplayTrack = new();
        public SessionTrackData PauseTrack = new();
        public SessionTrackData LoadingTrack = new();
    }

    [Serializable]
    public class SessionTrackData
    {
        public string TrackName = string.Empty;
        public float PlaybackTime = 0f;
    }
}
