using Asterodis.Settings;
using Services.AbstractFactoryService;
using Services.SettingsService;
using Zenject;

namespace Asterodis.GameBuilder
{
    public class StartLevel : IInitializable, ITickable
    {
        private readonly IGameContext gameContext;
        private readonly TickableManager tickableManager;
        private readonly ITask timer;
        private bool disposed;

        public StartLevel(
            ISettingsRepository settingsRepository,
            IGameContext gameContext,
            IAbstractFactory abstractFactory,
            TickableManager tickableManager)
        {
            this.gameContext = gameContext;
            this.tickableManager = tickableManager;
            var gameSetting = settingsRepository.Get<GameSettings>();
            timer = abstractFactory.Create<WhileTimer>(gameSetting.DelayLevelTimer);
        }

        public void Initialize()
        {
            if (disposed)
                return;
            
            tickableManager.Add(this);
        }

        public void Tick()
        {
            if (disposed || !timer.IsEnded)
                return;

            Dispose();
            gameContext.LevelUp();
        }
        
        private void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            tickableManager.Remove(this);
            timer?.Dispose();
        }
    }

    public class GameEnd : IInitializable
    {
        private readonly IGameContext gameContext;

        public GameEnd(IGameContext gameContext)
        {
            this.gameContext = gameContext;
        }
        
        public void Initialize()
        {
            throw new System.NotImplementedException();
        }
    }
}