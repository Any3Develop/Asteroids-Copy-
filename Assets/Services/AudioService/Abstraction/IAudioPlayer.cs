using System;
using UnityEngine;

namespace Services.AudioService
{
    public interface IAudioPlayer : IDisposable
    {
        float Volume { get; }
        bool IsMute { get; }
        event Action<IAudioPlayer> OnReleased;
        event Action OnCompleted;
            
        void SetMute(bool value);
        void SetVolume(float value);
        void Play(AudioClip value, bool loop);
        void PlayOneShot(AudioClip value, float volume);
        void Repeat();
        void Stop();
        void Release();
    }
}