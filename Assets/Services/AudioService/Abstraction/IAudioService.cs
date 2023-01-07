using System;

namespace Services.AudioService
{
    public interface IAudioService : IDisposable
    {
        float Volume { get; }
        bool IsMute { get; }
        
        void SetVolume(float value);
        void SetMute(bool value);
        IAudioHolder Play(string soundId, bool loop = false, float? volumeScale = null);
        void PlayOneShot(string soundId, float? volumeScale = null);
    }
}