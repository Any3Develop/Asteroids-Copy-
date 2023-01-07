using Zenject;

namespace Services.SceneService
{
    public class SceneServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<DefaultSceneService>()
                .AsSingle();
        }
    }
}