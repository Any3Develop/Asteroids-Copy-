using System;
using System.Collections.Generic;
using System.Linq;
using Asterodis.Entities;
using Asterodis.GameBuilder;
using Asterodis.Settings;
using Services.AudioService;
using Services.EntityService;
using Services.Extensions;
using Services.SettingsService;
using UnityEngine;
using Zenject;

namespace Asterodis.Audios
{
    public class AudioController : IInitializable, IDisposable, ILateTickable
    {
        private const string Once = "Once/";
        private const string Repeated = "Repeat/";
        private const string WhenDestory = "Destroy/";
        private const string WhenSpawn = "Spawn/";
        private const string WhenReqest = "Reqest/";
        private const string WhenLevelUp = "LevelUp/";
        private const string PlayerTag = "Player";
        private const string WhenBit = "Bit/";
        
        private readonly IAudioService audioService;
        private readonly IGameContext gameContext;
        private readonly ISettingsRepository settingsRepository;
        private readonly TickableManager tickableManager;
        private readonly Dictionary<string, string> audioMap;
        private readonly Dictionary<string, IAudioHolder> repeatedPlayers;
        private Dictionary<string, float> audioClipVolumes;
        private AudioSetting audioSetting;
        public bool disposed;
        private float levelTime;
        private float nextBitTime;
        private int bitId;
        
        public AudioController(
            IAudioService audioService, 
            IGameContext gameContext,
            ISettingsRepository settingsRepository,
            TickableManager tickableManager)
        {
            this.audioService = audioService;
            this.gameContext = gameContext;
            this.settingsRepository = settingsRepository;
            this.tickableManager = tickableManager;
            repeatedPlayers = new Dictionary<string, IAudioHolder>();
            
            // Audio map with commands
            audioMap = new Dictionary<string, string>
            {
                {$"{WhenDestory}Asteroid_0", $"{Once}bangLarge"},
                {$"{WhenDestory}Asteroid_1", $"{Once}bangMedium"},
                {$"{WhenDestory}Asteroid_2", $"{Once}bangSmall"},
                {$"{WhenDestory}PlayerAi_0", $"{Once}bangLarge"},
                {$"{WhenDestory}PlayerAi_1", $"{Once}bangSmall"},
                {$"{WhenDestory}Player",     $"{Once}bangSmall"},
                
                {$"{WhenSpawn}PlayerAi_0", $"{Repeated}{WhenDestory}saucerBig"},
                {$"{WhenSpawn}PlayerAi_1", $"{Repeated}{WhenDestory}saucerSmall"},
                {$"{WhenSpawn}GunPlayer",  $"{Once}fire"},
                {$"{WhenSpawn}GunEnemy",   $"{Once}fire1"},
                {$"{WhenSpawn}LaserPlayer",$"{Once}fire1"},
                {$"{WhenSpawn}LaserEnemy", $"{Once}fire1"},
                
                {$"{WhenLevelUp}{PlayerTag}", $"{Once}extraShip"},
                
                {$"{WhenReqest}{PlayerTag}{AudioAction.Move}", $"{Once}thrust"},
                {$"{WhenReqest}PlayerAi_0{AudioAction.Move}",  $"{Once}thrust"},
                {$"{WhenReqest}PlayerAi_1{AudioAction.Move}",  $"{Once}thrust"},
                
                {$"{WhenBit}1",  $"{Once}beat1"},
                {$"{WhenBit}2",  $"{Once}beat2"},
            };
        }

        public void Initialize()
        {
            audioSetting = settingsRepository.Get<AudioSetting>();
            audioClipVolumes = audioSetting.Pressets.ToDictionary(k => k.AudioName, v => v.ScaleVolume);
            gameContext.OnAudioReqested += OnAudioReqested;
            gameContext.OnLevelChanged += OnLevelChanged;
            gameContext.OnDestoryed += OnEntityDestoryed;
            gameContext.OnSpawn += OnEntitySpawned;
            gameContext.OnGameEnd += Dispose;
            audioService.SetVolume(audioSetting.Volume);
            audioService.SetMute(audioSetting.Mute);
            tickableManager.AddLate(this);
        }
        
        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;
            audioService.SetMute(true);
            audioClipVolumes.Clear();
            gameContext.OnAudioReqested -= OnAudioReqested;
            gameContext.OnLevelChanged -= OnLevelChanged;
            gameContext.OnDestoryed -= OnEntityDestoryed;
            gameContext.OnSpawn -= OnEntitySpawned;
            gameContext.OnGameEnd -= Dispose;
            repeatedPlayers.Keys.ToArray().ForEach(StopRepeated);
            repeatedPlayers.Clear();
            audioMap.Clear();
            tickableManager.RemoveLate(this);
            audioService.Dispose();
        }

        // custom bit audio
        public void LateTick()
        {
            if (gameContext.GameEnd)
                return;
            
            levelTime += Time.deltaTime;
            var bitTemp = Mathf.Min(audioSetting.BitTempMax, levelTime / audioSetting.BitLevelTime);
            if (nextBitTime < Time.time)
            {
                bitId = (bitId % 2) + 1;
                if (audioMap.TryGetValue(WhenBit + bitId, out var rawId))
                    AudioRouter(WhenBit, rawId);
                nextBitTime = Time.time + (1f - bitTemp);
            }
        }
        
        private void AudioRouter(string entityTag, string rawAudioId)
        {
            switch (GetCmd(rawAudioId))
            {
                case Once:
                    PlayAudioOneShot(FilterId(rawAudioId, Once));
                    return;
                
                case Repeated:
                    PlayRepeated(entityTag, FilterId(rawAudioId, Repeated));
                    break;
            }
        }

        private string FilterId(string rawId, string tag)
        {
            return rawId.Remove(0, tag.Length);
        }
        
        private string GetCmd(string rawId)
        {
            if (rawId.StartsWith(WhenReqest))
                return WhenReqest;
            
            if (rawId.StartsWith(WhenBit))
                return WhenBit;
            
            if (rawId.StartsWith(Repeated))
                return Repeated;

            if (rawId.StartsWith(Once))
                return Once;

            if (rawId.StartsWith(WhenDestory))
                return WhenDestory;

            if (rawId.StartsWith(WhenSpawn))
                return WhenSpawn;

            if (rawId.StartsWith(WhenLevelUp))
                return WhenLevelUp;

            return string.Empty;
        }

        private float? GetVolume(string audioId)
        {
            return audioClipVolumes.ContainsKey(audioId) 
                ? audioClipVolumes[audioId] 
                : default(float?);
        }

        private void PlayAudioOneShot(string id)
        {
            var volume = GetVolume(id);
            audioService.PlayOneShot(id, volume);
        }

        private void PlayRepeated(string entityTag, string rawAudioId)
        {
            var command = GetCmd(rawAudioId);
            var repeatId = command + entityTag;
            if (repeatedPlayers.ContainsKey(repeatId))
                return;
            
            var audioId = FilterId(rawAudioId, command);
            var volume = GetVolume(audioId);
            var audioHolder = audioService.Play(audioId, loop : true, volume);
            repeatedPlayers.Add(repeatId, audioHolder);
        }

        private void StopRepeated(string repeatId)
        {
            if (!repeatedPlayers.TryGetValue(repeatId, out var holder))
                return;

            holder?.Dispose();
            repeatedPlayers.Remove(repeatId);
        }
        
        private void OnLevelChanged()
        {
            bitId = 0;
            levelTime = 0f;
            nextBitTime = 0;
            var id = WhenLevelUp + PlayerTag;
            StopRepeated(id);
            
            if (audioMap.TryGetValue(id, out var rawAudioId))
                AudioRouter(PlayerTag, rawAudioId);
        }

        private void OnEntitySpawned(IEntity entity)
        {
            if (entity is not ITaggedEntity tagged)
                return;

            var id = WhenSpawn + tagged.Tag;
            StopRepeated(id);
            
            if (audioMap.TryGetValue(id, out var rawAudioId))
                AudioRouter(tagged.Tag, rawAudioId);
        }
        
        private void OnAudioReqested(IEntity entity, AudioAction action)
        {
            if (entity is not ITaggedEntity tagged)
                return;
            
            var id = WhenReqest + tagged.Tag + action;
            StopRepeated(id);
            
            if (audioMap.TryGetValue(id, out var rawAudioId))
                AudioRouter(tagged.Tag, rawAudioId);
        }
        
        private void OnEntityDestoryed(IEntity target, IEntity killer)
        {
            if (killer == null || killer == target || target is not ITaggedEntity tagged)
                return;

            var id = WhenDestory + tagged.Tag;
            StopRepeated(id);
            
            if (audioMap.TryGetValue(id, out var rawAudioId))
                AudioRouter(tagged.Tag, rawAudioId);
        }
    }
}