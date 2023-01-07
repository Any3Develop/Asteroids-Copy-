using Services.EntityService;
using Services.EntityService.Pool;
using Services.VFXService;

namespace Asterodis.Entities.VFX
{
    public interface IVfxSceneEntity : IVfx, ISceneEntity, IEntityPoolable {}
}