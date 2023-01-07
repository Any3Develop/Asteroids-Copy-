using Zenject;

namespace Services.PortalService
{
    public class PortalServiceInstaller : Installer<PortalServiceInstaller>
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<PortalService>()
                .AsSingle();
        }
    }
}