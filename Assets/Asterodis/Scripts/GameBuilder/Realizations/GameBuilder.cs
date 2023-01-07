using Services.AbstractFactoryService;

namespace Asterodis.GameBuilder
{
    public class GameBuilder : IGameBuilder
    {
        private readonly IAbstractFactory abstractFactory;
        public GameBuilder(IAbstractFactory abstractFactory)
        {
            this.abstractFactory = abstractFactory;
        }

        public IGame Build()
        {
            return abstractFactory.Create<AsteroidsGame>();
        }
    }
}