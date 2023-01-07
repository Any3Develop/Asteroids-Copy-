using Services.EntityService;

namespace Asterodis.Entities
{
    public interface IIndexedEntity : IEntity
    {
        int Index { get; }
        void SetIndex(int value);
    }
}