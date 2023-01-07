using System;

namespace Services.AudioService
{
    public interface IAudioHolder : IDisposable
    {
        float Volume { get; }
        bool IsMute { get; }
        
        event Action OnComplete;

        void SetMute(bool value);
        void Repeat();
        void Stop();
    }
}