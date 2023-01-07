using System;
using Zenject;

namespace Services.AudioService
{
    public class AudioHolder : IAudioHolder, IInitializable
    {
        private IAudioPlayer player;
        
        public float Volume => player?.Volume ?? 0f;
        
        public bool IsMute => player?.IsMute ?? true;
        
        public event Action OnComplete;

        public AudioHolder(IAudioPlayer player)
        {
            this.player = player;
        }

        public void Initialize()
        {
            player.OnCompleted += () => OnComplete?.Invoke();
            player.OnReleased += OnPlayerReleased;
        }

        public void Dispose()
        {
            if (player == null)
                return;

            var memo = player;
            player = null;
            memo?.Release();
            OnComplete = null;
        }

        public void SetMute(bool value)
        {
            player?.SetMute(value);
        }

        public void Repeat()
        {
            player?.Repeat();
        }

        public void Stop()
        {
            player?.Stop();
        }

        private void OnPlayerReleased(IAudioPlayer relesed)
        {
            if (player == null)
                return;

            Dispose();
        }
    }
}