using System;

namespace Asterodis.Entities.Statistics
{
    public class StatisticEntity : IStatisticEntity
    {
        public string Id { get; }

        public int Index { get; private set; }
        
        public string Title { get; private set;}
        
        public string Value { get; private set; }
        
        public event Action OnRefreshed;

        public StatisticEntity(string id)
        {
            Id = id;
        }

        public void SetTitle(string value)
        {
            Title = value;
        }

        public void SetValue(string value)
        {
            Value = value;
        }

        public void SetIndex(int value)
        {
            Index = value;
        }

        public void Refresh()
        {
            OnRefreshed?.Invoke();
        }

        public void Dispose()
        {
            OnRefreshed = null;
        }
    }
}