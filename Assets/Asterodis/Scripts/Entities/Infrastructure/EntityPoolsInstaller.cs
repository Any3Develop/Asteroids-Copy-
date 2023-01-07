using Asterodis.Entities.Players;
using Asterodis.Entities.VFX;
using Asterodis.Entities.Weapons;
using Services.EntityService.Pool;
using Zenject;

namespace Asterodis.Entities
{
    public class EntityPoolsInstaller : Installer<EntityPoolsInstaller>
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<EntityPoolCleaner>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<SceneEntityPool<IProjectileSceneEntity>>()
                .AsSingle()
                .WithArguments("ProjectileSceneEntityPool");
            
            Container
                .BindInterfacesTo<SceneEntityPool<IPlayerSceneEntity>>()
                .AsSingle()
                .WithArguments("PlayerSceneEntityPool");

            Container
                .BindInterfacesTo<SceneEntityPool<IVfxSceneEntity>>()
                .AsSingle()
                .WithArguments("VfxSceneEntityPool");
        }
    }
}