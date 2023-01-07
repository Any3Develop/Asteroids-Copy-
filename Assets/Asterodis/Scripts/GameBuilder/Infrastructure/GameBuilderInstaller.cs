using Zenject;

namespace Asterodis.GameBuilder
{
    public class GameBuilderInstaller : Installer<GameBuilderInstaller>
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<GameBuilder>()
                .AsSingle();

            Container
                .BindInterfacesTo<GameContext>()
                .AsCached();
        }
    }
}