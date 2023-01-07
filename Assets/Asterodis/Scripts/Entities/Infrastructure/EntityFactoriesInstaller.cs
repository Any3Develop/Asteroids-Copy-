using Asterodis.Entities.Players;
using Asterodis.Entities.Portals;
using Asterodis.Entities.Statistics;
using Asterodis.Entities.VFX;
using Asterodis.Entities.Weapons;
using Services.EntityService.Factory;
using Zenject;

namespace Asterodis.Entities
{
    public class EntityFactoriesInstaller: Installer<EntityFactoriesInstaller>
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<SceneEnityFactory<IProjectileSceneEntity>>()
                .AsSingle()
                .WithArguments("Entities/Projectails")
                .NonLazy();
            
            Container
                .BindInterfacesTo<SceneEnityFactory<IPlayerSceneEntity>>()
                .AsSingle()
                .WithArguments("Entities/Players")
                .NonLazy();
            
            Container
                .BindInterfacesTo<SceneEnityFactory<IWeaponSceneEntity>>()
                .AsSingle()
                .WithArguments("Entities/Weapons")
                .NonLazy();
            
            Container
                .BindInterfacesTo<SceneEnityFactory<IEdgePortalSceneEntity>>()
                .AsSingle()
                .WithArguments("Entities/Portals")
                .NonLazy();
            
            Container
                .BindInterfacesTo<SceneEnityFactory<IVfxSceneEntity>>()
                .AsSingle()
                .WithArguments("Entities/VFX")
                .NonLazy();
            
            Container
                .BindInterfacesTo<SceneEnityFactory<IAiTargetSceneEntity>>()
                .AsSingle()
                .WithArguments("Entities/AiTargets")
                .NonLazy();
            
            Container
                .BindInterfacesTo<SceneEnityFactory<IStatisticSceneEntity>>()
                .AsSingle()
                .WithArguments("Entities/Statistics/Prefabs")
                .NonLazy();
        }
    }
}