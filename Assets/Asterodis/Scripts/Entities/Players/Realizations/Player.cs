using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Asterodis.Audios;
using Asterodis.Entities.Modifiers;
using Asterodis.Entities.Movements;
using Asterodis.Entities.Statistics;
using Asterodis.Entities.VFX;
using Asterodis.Entities.Weapons;
using Asterodis.GameBuilder;
using Asterodis.Input;
using Asterodis.Settings;
using Services.AbstractFactoryService;
using Services.EntityService;
using Services.EntityService.Pool;
using Services.EntityService.Storage;
using Services.Extensions;
using Services.InputService;
using Services.LoggerService;
using Services.SettingsService;
using UnityEngine;
using Zenject;

namespace Asterodis.Entities.Players
{
    public class Player : IPlayer, IInitializable, IDamageHandler
    {
        private readonly IGameContext gameContext;
        private readonly IAbstractFactory abstractFactory;
        private readonly ISettingsRepository settingsRepository;
        private readonly IInputController<WeaponActions> inputWeapon;
        private readonly IInputController<MovementActions> inputMovement;
        private readonly IEntityStorage<IAiTargetSceneEntity> targetsStorage;
        private readonly IEntityStorage<IStatisticEntity> statisticStorage;
        private readonly IEntityPool<IPlayerSceneEntity> sceneEntityPool;
        private readonly IEntityPool<IVfxSceneEntity> vfxSceneEntityPool;
        private readonly Dictionary<string, CancellationTokenSource> firePocess;
        private readonly Dictionary<string, IWeapon> weapons;

        private IStatisticEntity healthStatistic;
        private Action onResetAnyInput;
        private IPlayerSceneEntity view;
        private PlayerSetting setting;
        private int currentHealth;
        private float lastVfxSpawned;

        public string Id { get; private set; }

        public Player(
            string id,
            IGameContext gameContext,
            IAbstractFactory abstractFactory,
            ISettingsRepository settingsRepository,
            IEntityPool<IPlayerSceneEntity> sceneEntityPool,
            IEntityPool<IVfxSceneEntity> vfxSceneEntityPool,
            IInputController<WeaponActions> inputWeapon,
            IInputController<MovementActions> inputMovement,
            IEntityStorage<IAiTargetSceneEntity> targetsStorage,
            IEntityStorage<IStatisticEntity> statisticStorage)
        {
            Id = id;
            this.gameContext = gameContext;
            this.sceneEntityPool = sceneEntityPool;
            this.vfxSceneEntityPool = vfxSceneEntityPool;
            this.abstractFactory = abstractFactory;
            this.settingsRepository = settingsRepository;
            this.inputWeapon = inputWeapon;
            this.inputMovement = inputMovement;
            this.targetsStorage = targetsStorage;
            this.statisticStorage = statisticStorage;
            weapons = new Dictionary<string, IWeapon>();
            firePocess = new Dictionary<string, CancellationTokenSource>();
        }

        public void Initialize()
        {
            setting = settingsRepository.Get<PlayerSetting>();
            currentHealth = setting.Health;

            Spawn();
            inputWeapon.Enable();
            inputMovement.Enable();
            inputWeapon.GetAll().ForEach(x =>
            {
                x.OnAnyStateChanged += OnWeaponeFire;
                x.OnAnyStateChanged += OnInputTriggered;
            });
            inputMovement.GetAll().ForEach(x => x.OnAnyStateChanged += OnInputTriggered);
            gameContext.OnLevelChanged += OnLevelChanged;
            var playerHealthStatistic = abstractFactory.Create<StatisticEntity>(Id);
            playerHealthStatistic.SetIndex(4);
            healthStatistic = playerHealthStatistic;
            playerHealthStatistic.OnRefreshed += () =>
            {
                playerHealthStatistic.SetTitle("Health");
                playerHealthStatistic.SetValue($"[{currentHealth}/{setting.Health}]");
            };
            statisticStorage.Add(playerHealthStatistic);
        }

        private void OnLevelChanged()
        {
            currentHealth = Mathf.Min(setting.Health, currentHealth + 1);
        }

        public void Dispose()
        {
            Despawn();
            statisticStorage.Remove(healthStatistic);
            healthStatistic?.Dispose();
            healthStatistic = null;
            onResetAnyInput?.Invoke();
            setting = null;
            onResetAnyInput = null;
            inputWeapon.GetAll().ForEach(x =>
            {
                x.OnAnyStateChanged -= OnWeaponeFire;
                x.OnAnyStateChanged -= OnInputTriggered;
            });
            gameContext.OnLevelChanged -= OnLevelChanged;
            inputMovement.GetAll().ForEach(x => x.OnAnyStateChanged -= OnInputTriggered);
            firePocess.Values.ForEach(x => x?.Cancel());
            weapons.Values.ForEach(x => x?.Dispose());
            firePocess.Clear();
            weapons.Clear();
            view = null;
            lastVfxSpawned = currentHealth = 0;
            Id = string.Empty;
        }

        public void HandleDamage(int value, IEntity killer)
        {
            if (gameContext.GameEnd)
                return;
            
            currentHealth -= value;

            if (currentHealth <= 0)
            {
                inputWeapon.Disable();
                inputMovement.Disable();
                gameContext.Destoyed(this, killer);
                Despawn();
                return;
            }

            gameContext.Destoyed(view, killer);
            Despawn();
            WaitSpawnAction();
        }

        public void AddWeapon(IProjectileWeapon weapon)
        {
            var actions = Enum.GetNames(typeof(WeaponActions));

            if (weapons.Count >= actions.Length)
            {
                DefaultLogger.Error($"Cant bind weapons more than {nameof(WeaponActions)} length : {actions.Length}");
                return;
            }

            var weaponId = actions[weapons.Count];

            if (weapons.ContainsKey(weaponId))
                throw new InvalidOperationException("Cant add same weapon twice");

            weapons.Add(weaponId, weapon);
            if (view is IWeaponSceneEntity weaponView)
                weapon.Initialize(Id, weaponView);
        }

        private void Spawn()
        {
            if (view != null || gameContext.GameEnd)
                return;

            inputWeapon.Disable(); // reset input
            var playerView = sceneEntityPool.Spawn<PlayerView>();
            view = playerView;
            playerView.SetTag(nameof(Player));
            playerView.SetOwnerId(Id);
            playerView.Container.position = Vector3.zero;
            playerView.Container.rotation = Quaternion.identity;

            if (playerView is IMovableSceneEntity movableView)
            {
                var movement = abstractFactory.Create<PlayerMovement>(Id, playerView.Container);
                movement.OnMove += () => PlayRokectVfxAsync(playerView.VfxPoint.position);
                movableView.SetMovement(movement);
            }

            if (playerView is IWeaponSceneEntity weaponView)
                weapons.Values.ForEach(x => x.Initialize(Id, weaponView));

            if (playerView is IDamageReceiverEntity damageReceiver)
                damageReceiver.SetDamageHandler(this);
            
            if (playerView is IAiTargetSceneEntity aiTarget)
                targetsStorage.Add(aiTarget);

            var modifier = abstractFactory.Create<PlayerImmortalModifier>();
            modifier.Apply(Id);
            modifier.OnComplete(() => onResetAnyInput -= modifier.Dispose);
            onResetAnyInput += modifier.Dispose;
            inputWeapon.Enable();
            gameContext.Spawned(view);
        }

        private void Despawn()
        {
            if (view == null)
                return;

            PlayDestoryVfxAsync();
            
            if (view is IAiTargetSceneEntity aiTarget)
                targetsStorage.Remove(aiTarget);
            
            weapons.Values.ForEach(x => x?.Release());
            sceneEntityPool.DespawnLate(view, () =>
            {
                lastVfxSpawned = 0;
                view = null;
            });
        }

        private void WaitSpawnAction()
        {
            var task = abstractFactory.Create<WhileActionOrTimer>(setting.RespawnTime);
            task.OnComplete(Spawn);
            gameContext.AddTask(task);
        }

        private void PlayRokectVfxAsync(Vector3 vfxSpawnPoint)
        {
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
            var vfxView = vfxSceneEntityPool.Spawn<DestroyVfxView>(nameof(Player) + nameof(DestroyVfxView));
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

        private void OnWeaponeFire(IInputContext context)
        {
            if (gameContext.GameEnd)
                return;
            
            if (string.IsNullOrEmpty(context.Id)
                || !weapons.TryGetValue(context.Id, out var weapon))
                return;

            if (weapon is not IProjectileWeapon projectileWeapon)
                return;

            if (context.Performed && !firePocess.ContainsKey(context.Id))
            {
                var cancelationSource = new CancellationTokenSource();
                UpdateAsync(cancelationSource.Token).ConfigureAwait(true);
                firePocess.Add(context.Id, cancelationSource);
                return;
            }

            if (context.Canceled && firePocess.TryGetValue(context.Id, out var tokenSource))
            {
                tokenSource?.Cancel();
                tokenSource?.Dispose();
                firePocess.Remove(context.Id);
            }

            async Task UpdateAsync(CancellationToken token)
            {
                while (!token.IsCancellationRequested && Application.isPlaying && !gameContext.GameEnd)
                {
                    projectileWeapon.Fire();
                    await Task.Yield();
                }
            }
        }
        
        private void OnInputTriggered(IInputContext context)
        {
            onResetAnyInput?.Invoke();
            onResetAnyInput = null;
        }
    }
}