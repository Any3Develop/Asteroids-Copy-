using System;
using System.Collections.Generic;
using System.Linq;
using Asterodis.Entities.Weapons;
using Asterodis.GameBuilder;
using Asterodis.Settings;
using Services.EntityService.Factory;
using Services.Extensions;
using Services.SettingsService;
using UnityEngine;
using Zenject;

namespace Asterodis.Entities.Players
{
    public class PlayerUniverse : IPlayer, IInitializable
    {
        private readonly Camera gameCamera;
        private readonly IGameContext gameContext;
        private readonly ISceneEntityFactory<IWeaponSceneEntity> sceneEntityFactory;
        private readonly ISettingsRepository settingsRepository;
        private readonly List<IWeaponSceneEntity> weaponViews;
        
        private WeaponSetting projectileSettings;
        private PlayerUniverseSetting settings;
        private GameSettings gameSettings;
        private IProjectileWeapon weapon;

        public string Id { get; private set; }

        public PlayerUniverse(
            string id,
            Camera gameCamera,
            IGameContext gameContext,
            ISceneEntityFactory<IWeaponSceneEntity> sceneEntityFactory,
            ISettingsRepository settingsRepository)
        {
            Id = id;
            this.gameCamera = gameCamera;
            this.gameContext = gameContext;
            this.sceneEntityFactory = sceneEntityFactory;
            this.settingsRepository = settingsRepository;
            weaponViews = new List<IWeaponSceneEntity>();
        }

        public void Initialize()
        {
            gameContext.OnLevelChanged += OnLevelChanged;
            settings = settingsRepository.Get<PlayerUniverseSetting>(GetType().Name);
            gameSettings = settingsRepository.Get<GameSettings>();
        }

        public void Dispose()
        {
            gameContext.OnLevelChanged -= OnLevelChanged;
            weaponViews.ForEach(x=> x?.Dispose());
            weaponViews.Clear();
            weapon?.Dispose();
            projectileSettings = null;
            gameSettings = null;
            settings = null;
            weapon = null;
            Id = string.Empty;
        }

        public void AddWeapon(IWeapon weapon)
        {
            if (weapon is not IProjectileWeapon projectileWeapon)
                throw new InvalidOperationException($"{nameof(PlayerUniverse)} - can use only : " +
                                                    $"{nameof(IProjectileWeapon)}");
            
            this.weapon?.Dispose();
            this.weapon = projectileWeapon;
            projectileSettings = settingsRepository.Get<WeaponSetting>(weapon.GetType().Name);
        }

        private void OnLevelChanged()
        {
            var perLevelAttackCount = Mathf.FloorToInt((gameContext.Level - 1) * settings.DifficultFactor);
            var projectileCount = settings.InitAttackCount + perLevelAttackCount;
            var positions = GeneratePositions(projectileCount, projectileSettings.ProjectileSize);

            for (var i = 0; i < projectileCount; i++)
            {
                if (projectileCount - weaponViews.Count > 0)
                {
                    var weaponView = sceneEntityFactory.Create<DefaultWeaponView>();
                    weaponView.SetOwnerId(Id);
                    weaponViews.Add(weaponView);
                }

                weaponViews[i].Container.position = positions[i];
            }

            weapon.Initialize(Id, weaponViews.ToArray());
            projectileCount.Enumerate(_ => weapon.Fire());
        }
        
        private Vector3[] GeneratePositions(int count, float margin = 0)
        {
            var worldBounds = gameCamera.CalculateWorldBounds(margin, gameSettings.SceneDepth);
            return count
                .Select(i => VectorExtensions.CalculatePointOnBoundingEdges((float) i / count, worldBounds))
                .ToArray();
        }
    }
}