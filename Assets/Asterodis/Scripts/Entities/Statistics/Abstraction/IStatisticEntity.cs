using System;

namespace Asterodis.Entities.Statistics
{
    public interface IStatisticEntity : IIndexedEntity, IDisposable
    {
        string Title { get; }
        string Value { get; }
        void Refresh();
    }
}