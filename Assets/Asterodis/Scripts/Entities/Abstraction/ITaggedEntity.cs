using Services.EntityService;

namespace Asterodis.Entities
{
    public interface ITaggedEntity : IEntity
    {
        string Tag { get; }
    }
}