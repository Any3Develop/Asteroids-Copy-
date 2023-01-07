using Zenject;

namespace Services.ImageLoaderService
{
    public class ImageLoaderInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<ImageLoader>()
                .AsSingle();
        }
    }
}