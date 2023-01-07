using Zenject;

namespace Services.SettingsService
{
    public class SettingsServiceInstaller : MonoInstaller<SettingsServiceInstaller>
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<SettingRepository>()
                .AsSingle()
                .NonLazy();
        }
    }
}