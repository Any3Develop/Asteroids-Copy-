using Zenject;

namespace Services.SaveService
{
    public class SaveServiceInstaller : MonoInstaller<SaveServiceInstaller>
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<SaveLocalService>()
                .AsSingle()
                .NonLazy();
        }
    }
}