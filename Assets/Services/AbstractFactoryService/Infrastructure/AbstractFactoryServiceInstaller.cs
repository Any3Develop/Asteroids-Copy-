using Zenject;

namespace Services.AbstractFactoryService
{
    public class AbstractFactoryServiceInstaller : Installer<AbstractFactoryServiceInstaller>
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<AbstractFactory>()
                .AsSingle();
        }
    }
}