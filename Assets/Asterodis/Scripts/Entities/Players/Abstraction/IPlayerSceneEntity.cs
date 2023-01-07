using Services.EntityService;
using Services.EntityService.Pool;


namespace Asterodis.Entities.Players
{
    public interface IPlayerSceneEntity : IEntityPoolable, ISceneEntity
    {
    }
}