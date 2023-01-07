using System.Collections.Generic;
using Services.AbstractFactoryService;
using Services.Extensions;
using Services.LoggerService;

namespace Services.AudioService
{
    public class AudioService : IAudioService
    {
        private readonly IClipStorage storage;
        private readonly IAudioPlayerFactory audioPlayerFactory;
        private readonly IAbstractFactory abstractFactory;
        private readonly List<IAudioPlayer> activePlayers;
        private readonly Queue<IAudioPlayer> poolPlayers;
        private IAudioPlayer playerOneShot;
        
        public float Volume { get; private set; }
        public bool IsMute { get; private set; }
        
        public AudioService(IClipStorage storage, IAudioPlayerFactory audioPlayerFactory, IAbstractFactory abstractFactory)
        {
            this.storage = storage;
            this.audioPlayerFactory = audioPlayerFactory;
            this.abstractFactory = abstractFactory;
            poolPlayers = new Queue<IAudioPlayer>();
            activePlayers = new List<IAudioPlayer>();
        }

        public void PlayOneShot(string soundId, float? volumeScale = null)
        {
            if (!storage.Contains(soundId))
            {
                DefaultLogger.Error($"Target sound with id : {soundId} does not exist");
                return;
            }
            
            playerOneShot ??= poolPlayers.Count > 0
                ? poolPlayers.Dequeue()
                : audioPlayerFactory.Create(); 
            var clip = storage.Get(soundId);
            playerOneShot.SetMute(IsMute);
            playerOneShot.PlayOneShot(clip, volumeScale.HasValue ? Volume * volumeScale.Value : Volume);
        }

        public IAudioHolder Play(string soundId, bool loop = false, float? volumeScale = null)
        {
            if (!storage.Contains(soundId))
            {
                DefaultLogger.Error($"Target sound with id : {soundId} does not exist");
                return default;
            }
            
            if (poolPlayers.Count == 0)
                poolPlayers.Enqueue(audioPlayerFactory.Create());

            var clip = storage.Get(soundId);
            var player = poolPlayers.Dequeue();
            activePlayers.Add(player);
            
            player.OnReleased += OnAudioPlayerReleased;
            player.SetVolume(volumeScale.HasValue ? Volume * volumeScale.Value : Volume);
            player.SetMute(IsMute);
            player.Play(clip, loop);
            return abstractFactory.Create<AudioHolder>(player);
        }

        public void SetVolume(float value)
        {
            Volume = value;
            activePlayers.ToArray().ForEach(x=> x.SetVolume(value));
            poolPlayers.ToArray().ForEach(x=> x.SetVolume(value));
        }

        public void SetMute(bool value)
        {
            IsMute = value;
            activePlayers.ToArray().ForEach(x=> x.SetMute(value));
            poolPlayers.ToArray().ForEach(x=> x.SetMute(value));
        }
        
        private void OnAudioPlayerReleased(IAudioPlayer player)
        {
            activePlayers.Remove(player);
            player.OnReleased -= OnAudioPlayerReleased;
            if (!poolPlayers.Contains(player))
                poolPlayers.Enqueue(player);
        }

        public void Dispose()
        {
            playerOneShot?.Dispose();
            playerOneShot = null;
            activePlayers.ToArray().ForEach(x=> x?.Dispose());
            poolPlayers.ToArray().ForEach(x=> x?.Dispose());
            activePlayers.Clear();
            poolPlayers.Clear();
        }
    }
}