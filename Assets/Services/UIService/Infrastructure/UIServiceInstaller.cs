using Zenject;

namespace Services.UIService
{
    public class UIServiceInstaller : MonoInstaller<UIServiceInstaller>
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<UIRoot>()
                .FromComponentInNewPrefabResource("UIWindows/UIRoot")
                .AsSingle()
                .NonLazy();

            Container
                .Bind<IUIService>()
                .To<UIService>()
                .AsSingle()
                .NonLazy();
        }
    }
}