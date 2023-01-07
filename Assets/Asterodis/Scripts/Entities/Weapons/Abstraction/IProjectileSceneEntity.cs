using Services.EntityService.Pool;

namespace Asterodis.Entities.Weapons
{
    public interface IProjectileSceneEntity : IEntityPoolable, IContactableSceneEntity, IMovableSceneEntity
    {
    }
}