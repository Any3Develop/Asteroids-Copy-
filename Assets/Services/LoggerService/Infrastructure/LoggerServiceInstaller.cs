using Zenject;

namespace Services.LoggerService
{
    public class LoggerServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<DebugLogger>()
                .AsSingle()
                .NonLazy();
        }
    }
}