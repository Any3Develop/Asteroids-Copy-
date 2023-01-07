using Services.EntityService;

namespace Asterodis.Entities
{
    public interface IDamageReceiverEntity : IEntity
    {
        void SetDamageHandler(IDamageHandler value);
        void ReceiveDamage(int value, IEntity killer);
    }
}