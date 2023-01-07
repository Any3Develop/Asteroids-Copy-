using Asterodis.Entities.Portals;
using Zenject;

namespace Asterodis.Entities
{
    public class PortalsInstaller : Installer<PortalsInstaller>
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<ScreenPortalsProvider>()
                .AsSingle()
                .NonLazy();
        }
    }
}