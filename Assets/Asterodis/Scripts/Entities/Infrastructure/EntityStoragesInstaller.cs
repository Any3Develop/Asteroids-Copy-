using Asterodis.Entities.Players;
using Asterodis.Entities.Statistics;
using Services.EntityService.Storage;
using Zenject;

namespace Asterodis.Entities
{
    public class EntityStoragesInstaller : Installer<EntityStoragesInstaller>
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<EntityStorage<IAiTargetSceneEntity>>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<EntityStorage<IStatisticEntity>>()
                .AsSingle();
        }
    }
}