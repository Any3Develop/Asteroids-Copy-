using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asterodis.Audios;
using Asterodis.Entities.Movements;
using Asterodis.Entities.VFX;
using Asterodis.Entities.Weapons;
using Asterodis.GameBuilder;
using Asterodis.Settings;
using Services.AbstractFactoryService;
using Services.EntityService;
using Services.EntityService.Factory;
using Services.EntityService.Pool;
using Services.EntityService.Storage;
using Services.Extensions;
using Services.LoggerService;
using Services.SettingsService;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Asterodis.Entities.Players
{
    public class PlayerAi : IPlayer, IInitializable, IDamageHandler, ILateTickable
    {

        private readonly Camera gameCamera;
        private readonly IGameContext gameContext;
        private readonly IAbstractFactory abstractFactory;
        private readonly ISettingsRepository settingsRepository;
        private readonly IEntityPool<IPlayerSceneEntity> sceneEntityPool;
        private readonly IEntityPool<IVfxSceneEntity> vfxSceneEntityPool;
        private readonly IEntityStorage<IAiTargetSceneEntity> targetStorage;
        private readonly ISceneEntityFactory<IAiTargetSceneEntity> targetfactory;
        private readonly Dictionary<string, CancellationTokenSource> firePocess;
        private readonly Dictionary<string, IWeapon> weapons;
        private readonly TickableManager tickableManager;
        
        private IPlayerSceneEntity view;
        private GameSettings gameSettings;
        private PlayerAiSetting setting;
        private float lastVfxSpawned;
        private int currentHealth;
        private int appearanceCount;

        public string Id { get; private set; }

        public PlayerAi(
            string id,
            Camera gameCamera,
            IGameContext gameContext,
            IAbstractFactory abstractFactory,
            ISettingsRepository settingsRepository,
            IEntityPool<IPlayerSceneEntity> sceneEntityPool,
            IEntityPool<IVfxSceneEntity> vfxSceneEntityPool,
            IEntityStorage<IAiTargetSceneEntity> targetStorage,
            ISceneEntityFactory<IAiTargetSceneEntity> targetfactory,
            TickableManager tickableManager)
        {
            Id = id;
            this.gameCamera = gameCamera;
            this.gameContext = gameContext;
            this.sceneEntityPool = sceneEntityPool;
            this.vfxSceneEntityPool = vfxSceneEntityPool;
            this.targetStorage = targetStorage;
            this.targetfactory = targetfactory;
            this.tickableManager = tickableManager;
            this.abstractFactory = abstractFactory;
            this.settingsRepository = settingsRepository;
            weapons = new Dictionary<string, IWeapon>();
            firePocess = new Dictionary<string, CancellationTokenSource>();
        }

        public void Initialize()
        {
            gameSettings = settingsRepository.Get<GameSettings>();
            setting = settingsRepository.Get<PlayerAiSetting>();
            gameContext.OnLevelChanged += OnLevelChanged;
            tickableManager.AddLate(this);
        }

        public void Dispose()
        {
            Despawn();
            gameContext.OnLevelChanged -= OnLevelChanged;
            var targets = targetStorage.GetAll(Id);
            targetStorage.Remove(Id);
            targets.ForEach(x=> x?.Dispose());
            firePocess.Values.ForEach(x=> x?.Cancel());
            weapons.Values.ForEach(x => x?.Dispose());
            firePocess.Clear();
            weapons.Clear();
            setting = null;
            view = null;
            Id = string.Empty;
            currentHealth = 0;
            tickableManager.RemoveLate(this);
        }

        public void HandleDamage(int value, IEntity killer)
        {
            if (gameContext.GameEnd)
                return;
            
            currentHealth -= value;
            
            if (currentHealth > 0)
                return;

            gameContext.Destoyed(view, killer);
            PlayDestoryVfxAsync();
            Despawn();
        }

        public void AddWeapon(IWeapon weapon)
        {
            var weaponId = weapon.GetType().Name;
            if (weapons.ContainsKey(weaponId))
                throw new InvalidOperationException("Cant add same weapon twice");
            
            weapons.Add(weaponId, weapon);
            
            if (view is IWeaponSceneEntity weaponView)
                weapon.Initialize(weapon.GetType().Name, weaponView);
        }
        
        public void LateTick()
        {
            if (gameContext.GameEnd)
                return;
            
            if (view != null || appearanceCount >= setting.PerLevelAppearances)
                return;
            
            var progressStep = 0.99f / setting.PerLevelAppearances;
            var currentAppearance = (appearanceCount + 1) * progressStep;
            
            if(currentAppearance <= gameContext.TasksProgress)
                Spawn();
        }

        private void Spawn()
        {
            if (view != null || gameContext.GameEnd)
                return;

            appearanceCount++;
            currentHealth = setting.Health;
            var spawnPoint = targetStorage.GetExcept(gameContext.PlayerId).Shuffle().First();
            var variation = Random.Range(0, setting.VariationsCount);
            var playerView = sceneEntityPool.Spawn<PlayerAIView>(nameof(PlayerAIView) + variation);
            view = playerView;
            playerView.SetTag(nameof(PlayerAi)+ $"_{variation}");
            playerView.SetOwnerId(Id);
            playerView.Container.position = spawnPoint.Container.position;
            playerView.Container.rotation = spawnPoint.Container.rotation;

            if (playerView is IMovableSceneEntity movableView)
            {
                var movement = abstractFactory.Create<PlayerAiMovement>(Id, playerView.Container, variation);
                movement.OnMove += () => PlayRokectVfxAsync(playerView.VfxPoint.position);
                movement.OnManeuversCompleted += OnManeuversCompleted;
                movableView.SetMovement(movement);
            }

            if (playerView is IWeaponSceneEntity weaponView)
                weapons.Values.ForEach(x => x.Initialize(Id, weaponView));

            if (playerView is IDamageReceiverEntity damageReceiver)
                damageReceiver.SetDamageHandler(this);
            
            weapons.ForEach(item => OnWeaponeFire(item.Key, true));
            var task = abstractFactory.Create<WhileAllEntityDestoyed>(Id, 1, view.GetType());
            gameContext.AddTask(task);
            gameContext.Spawned(view);
        }

        private void Despawn()
        {
            if (view == null)
                return;
            
            weapons.ForEach(item => OnWeaponeFire(item.Key, false));
            weapons.Values.ForEach(x=> x?.Release());
            sceneEntityPool.DespawnLate(view, () =>
            {
                lastVfxSpawned = 0;
                view = null;
            });
        }

        private void PlayRokectVfxAsync(Vector3 vfxSpawnPoint)
        {
            if (gameContext.GameEnd)
                return;
            
            if (lastVfxSpawned > Time.time)
                return;

            lastVfxSpawned = Time.time + 0.05f;
            var vfxView = vfxSceneEntityPool.Spawn<RocketBoosterVfxView>();
            vfxView.SetPosition(vfxSpawnPoint, Space.World);
            vfxView.SetRotation(view.Container.rotation, Space.World);
            gameContext.ReqestAudio(view, AudioAction.Move);
            HandleVfxAsync();

            async void HandleVfxAsync()
            {
                await vfxView.PlayAsync();
                await vfxView.StopAsync();
                vfxSceneEntityPool.Despawn(vfxView);
            }
        }

        private void PlayDestoryVfxAsync()
        {
            if (gameContext.GameEnd)
                return;
            
            var vfxView = vfxSceneEntityPool.Spawn<DestroyVfxView>(nameof(DestroyVfxView));
            vfxView.SetPosition(view.Container.position, Space.World);
            vfxView.SetRotation(view.Container.rotation, Space.World);
            HandleVfxAsync();
            
            async void HandleVfxAsync()
            {
                await vfxView.PlayAsync();
                await vfxView.StopAsync();
                vfxSceneEntityPool.Despawn(vfxView);
            }
        }
        
        private void OnLevelChanged()
        {
            if (gameContext.GameEnd)
                return;
            
            var worldBounds = gameCamera.CalculateWorldBounds(depth: gameSettings.SceneDepth);
            var targetsCount = setting.SpawnPointCount;
            var spawnTargets = targetStorage.GetAll(Id).ToList();
            
            for (var i = 0; i < targetsCount; i++)
            {
                if (spawnTargets.Count < targetsCount)
                {
                    var target = targetfactory.Create<AiTargetView>();
                    target.SetOwnerId(Id);
                    spawnTargets.Add(target);
                    targetStorage.Add(target);
                }

                var aiTargetView = spawnTargets[i];
                var point = VectorExtensions.CalculatePointOnBoundingEdges((float) i / targetsCount, worldBounds);
                aiTargetView.Container.position = point;
            }
            appearanceCount = 0;
        }
        
        private void OnManeuversCompleted()
        {
            gameContext.Destoyed(view, this);
            Despawn();
        }

        private void OnWeaponeFire(string weaponeId, bool performed)
        {
            if (gameContext.GameEnd)
                return;
            
            if (!weapons.TryGetValue(weaponeId, out var weapon))
                return;
            
            if (weapon is not IProjectileWeapon projectileWeapon)
                return;

            if (performed && !firePocess.ContainsKey(Id))
            {
                var cancelationSource = new CancellationTokenSource();
                UpdateAsync(cancelationSource.Token).ConfigureAwait(true);
                firePocess.Add(Id, cancelationSource);
                return;
            }

            if (!performed && firePocess.TryGetValue(Id, out var tokenSource))
            {
                tokenSource?.Cancel();
                tokenSource?.Dispose();
                firePocess.Remove(Id);
            }
            
            async Task UpdateAsync(CancellationToken token)
            {
                if (view is not IWeaponSceneEntity sceneEntity)
                    throw new OperationCanceledException($"{nameof(IWeaponSceneEntity)} not found");

                var rotationAngle = 0f;
                while (!token.IsCancellationRequested && Application.isPlaying && !gameContext.GameEnd)
                {
                    rotationAngle += setting.AimRotationSpeed * Time.deltaTime;
                    sceneEntity.Aim.RotateAround(sceneEntity.Container.position, Vector3.forward, rotationAngle);
                    projectileWeapon.Fire();
                    await Task.Yield();
                }
            }
        }
    }
}