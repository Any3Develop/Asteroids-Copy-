using Asterodis.Entities.Movements;
using Services.EntityService;

namespace Asterodis.Entities
{
    public interface IMovableSceneEntity : ISceneEntity
    {
        void SetMovement(IMovement value);
    }
}