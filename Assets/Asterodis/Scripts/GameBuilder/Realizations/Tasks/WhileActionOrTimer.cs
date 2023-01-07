using System;
using Services.AbstractFactoryService;
using Zenject;

namespace Asterodis.GameBuilder
{
    public class WhileActionOrTimer : ITask, ITickable, IInitializable
    {
        private readonly TickableManager tickableManager;
        private readonly ITask timer;
        private Action onComplete;

        public bool IsEnded => timer.IsEnded;

        public bool Disposed { get; private set; }

        public float Progress => IsEnded ? 1f : timer.Progress;


        public WhileActionOrTimer(
            float actionTimer,
            IAbstractFactory abstractFactory, 
            TickableManager tickableManager)
        {
            this.tickableManager = tickableManager;
            timer = abstractFactory.Create<WhileTimer>(actionTimer);
        }

        public void OnComplete(Action value)
        {
            onComplete += () => value?.Invoke();
        }
        
        public void Initialize()
        {
            tickableManager.Add(this);
        }
        
        public void Dispose()
        {
            if (Disposed)
                return;

            Disposed = true;
            timer?.Dispose();
            onComplete?.Invoke();
            onComplete = null;
            tickableManager.Remove(this);
        }

        public void Tick()
        {
            if (!IsEnded)
                return;
            
            Dispose();
        }
    }
}