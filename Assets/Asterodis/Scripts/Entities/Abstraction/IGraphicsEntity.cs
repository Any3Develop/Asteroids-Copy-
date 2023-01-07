using Services.EntityService;

namespace Asterodis.Entities
{
    public interface IGraphicsEntity : IEntity
    {
        float Alpha { get; }
        void SetAlpha(float value);
    }
}