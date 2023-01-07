using Services.EntityService;

namespace Asterodis.Entities
{
    public interface IDamageHandler : IEntity
    {
        void HandleDamage(int value, IEntity killer);
    }
}