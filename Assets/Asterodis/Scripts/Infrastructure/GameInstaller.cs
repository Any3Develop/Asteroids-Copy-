using Asterodis.Audios;
using Asterodis.Entities;
using Asterodis.Entities.Portals;
using Asterodis.GameBuilder;
using Asterodis.Input;
using DG.Tweening;
using Services.AbstractFactoryService;
using Services.AudioService;
using Services.PortalService;
using Zenject;

namespace Asterodis
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            AbstractFactoryServiceInstaller.Install(Container);
            EntityFactoriesInstaller.Install(Container);
            EntityStoragesInstaller.Install(Container);
            EntityPoolsInstaller.Install(Container);
            GameBuilderInstaller.Install(Container);
            SoundServiceInstaller.Install(Container);
            PortalServiceInstaller.Install(Container);
            PortalsInstaller.Install(Container);
            InputInstaller.Install(Container);
            DOTween.SetTweensCapacity(1250, 125);
            Container
                .BindInterfacesTo<Launcher>()
                .AsSingle()
                .NonLazy();
        }
    }
}