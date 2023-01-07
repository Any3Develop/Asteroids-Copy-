using System;
using System.Collections.Generic;
using System.Linq;
using Asterodis.Audios;
using Asterodis.Entities;
using Asterodis.Entities.Players;
using Asterodis.Entities.Statistics;
using Asterodis.Entities.VFX;
using Asterodis.Entities.Weapons;
using Asterodis.Settings;
using Services.AbstractFactoryService;
using Services.EntityService;
using Services.EntityService.Pool;
using Services.Extensions;
using Services.LoggerService;
using Services.SettingsService;
using UnityEngine;
using Zenject;

namespace Asterodis.GameBuilder
{
    public class AsteroidsGame : IGame, ILateTickable
    {
        private readonly IGameContext gameContext;
        private readonly IAbstractFactory abstractFactory;
        private readonly ISettingsRepository settingsRepository;
        private readonly IEntityPool<IVfxSceneEntity> vfxSceneEntityPool;
        private readonly IEntityPoolCleaner entityPoolCleaner;
        private readonly TickableManager tickableManager;

        private StatisticsController statisticsController;
        private AudioController audioController;
        private Dictionary<string, int> scores;
        private PlayerAiSetting aiSetting;
        private GameSettings settings;
        private List<IPlayer> players;
        private bool gameStarted;
        
        public event Action OnRestartRequired;
        
        public AsteroidsGame(
            IGameContext gameContext,
            IAbstractFactory abstractFactory,
            ISettingsRepository settingsRepository,
            IEntityPool<IVfxSceneEntity> vfxSceneEntityPool,
            IEntityPoolCleaner entityPoolCleaner,
            TickableManager tickableManager)
        {
            this.gameContext = gameContext;
            this.abstractFactory = abstractFactory;
            this.settingsRepository = settingsRepository;
            this.vfxSceneEntityPool = vfxSceneEntityPool;
            this.entityPoolCleaner = entityPoolCleaner;
            this.tickableManager = tickableManager;
        }

        public void Start()
        {
            if (gameStarted)
                throw new InvalidOperationException("Game already started!");

            gameStarted = true;
            gameContext.Reset();
            audioController = abstractFactory.Create<AudioController>();
            statisticsController = abstractFactory.Create<StatisticsController>();
            aiSetting = settingsRepository.Get<PlayerAiSetting>();
            settings = settingsRepository.Get<GameSettings>();
            scores = settings.Scores.ToDictionary(x => x.Tag, x => x.Quantity);
            gameContext.OnTasksCompleted += StartLevel;
            gameContext.OnDestoryed += OnEnityDestoryed;
            tickableManager.AddLate(this);
            InitializePlayers();
            StartLevel();
        }

        public void Stop()
        {
            if (!gameStarted)
                return;
            
            gameStarted = false;
            tickableManager.RemoveLate(this);
            statisticsController?.Dispose();
            audioController?.Dispose();
            players.ForEach(x => x?.Dispose());
            entityPoolCleaner.ClearAllPools();
            scores.Clear();
            players.Clear();
            gameContext.Reset();
            OnRestartRequired = null;
            statisticsController = null;
            audioController = null;
            aiSetting = null;
            settings = null;
            players = null;
            scores = null;
        }
        
        public void LateTick()
        {
            if (gameContext.GameEnd)
                return;
            
            if (gameContext.TaskCount <= 0)
                return;

            var tasks = gameContext.Tasks.ToArray();
            if (tasks.All(x => x.IsEnded))
                tasks.ForEach(gameContext.CompleteTask);
        }

        private void InitializePlayers()
        {
            if (!gameStarted)
                return;
            
            players = new List<IPlayer>();
            
            gameContext.PlayerId = Guid.NewGuid().ToString();
            var player = abstractFactory.Create<Player>(gameContext.PlayerId);
            player.AddWeapon(abstractFactory.Create<GunWeapon>());
            player.AddWeapon(abstractFactory.Create<LaserWeapon>());
            players.Add(player);
            
            var universePlayerId = Guid.NewGuid().ToString(); // family Id for all AIs
            var universePlayer = abstractFactory.Create<PlayerUniverse>(universePlayerId);
            universePlayer.AddWeapon(abstractFactory.Create<AsteroidsWeapon>());
            players.Add(universePlayer);

            var aiPlayerId = aiSetting.AIEnvironmentCollision
                ? Guid.NewGuid().ToString() 
                : universePlayerId;
            
            var playerAi = abstractFactory.Create<PlayerAi>(aiPlayerId);
            playerAi.AddWeapon(abstractFactory.Create<GunWeapon>());
            playerAi.AddWeapon(abstractFactory.Create<CollisionWeapon>());
            players.Add(playerAi);
        }

        private void StartLevel()
        {
            if (gameContext.GameEnd)
                return;
            
            abstractFactory.Create<StartLevel>();
        }

        private void OnEnityDestoryed(IEntity target, IEntity killer)
        {
            if (gameContext.GameEnd)
                return;
            
            if (TryGameEnd(target))
                return;
            
            if (killer?.Id != gameContext.PlayerId)
                return;
            
            if (target is not ITaggedEntity entityTagged)
                return;

            if (string.IsNullOrEmpty(entityTagged.Tag))
            {
                DefaultLogger.Error($"[{nameof(AsteroidsGame)}].OnEnityDestoryed : " +
                                    $"Tagged entity : {entityTagged.GetType().Name} with empty tag property!!");
                return;
            }

            if (!scores.TryGetValue(entityTagged.Tag, out var score))
                return;
            
            var totalScore = Mathf.CeilToInt(score * (gameContext.Level * settings.ScoreMultiplier));
            gameContext.AddScore(totalScore);
            PlayTextScoreAsync(target, totalScore.ToString());
        }

        private bool TryGameEnd(IEntity value)
        {
            if (value is not IPlayer || gameContext.PlayerId != value.Id)
                return false;

            gameContext.SetGameEnd(true);
            OnRestartRequired?.Invoke();
            return true;
        }
        
        private void PlayTextScoreAsync(IEntity target, string scoreText)
        {
            if (target is not ISceneEntity sceneEntity) 
                return;
                    
            var textVfxView = vfxSceneEntityPool.Spawn<TextVfxView>();
            textVfxView.SetPosition(sceneEntity.Container.position, Space.World);
            textVfxView.SetText(scoreText);
            HandleVfxAsync();

            async void HandleVfxAsync()
            {
                await textVfxView.PlayAsync();
                await textVfxView.StopAsync();
                vfxSceneEntityPool.Despawn(textVfxView);
            }
        }
    }
}