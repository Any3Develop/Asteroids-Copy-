using System;

namespace Asterodis.GameBuilder
{
    public interface ITask : IDisposable
    {
        bool IsEnded { get; }
        bool Disposed { get; }
        float Progress { get; }
    }
}