using System;

namespace Asterodis.GameBuilder
{
    public interface IGame
    {
        event Action OnRestartRequired;
        void Start();
        void Stop();
    }
}