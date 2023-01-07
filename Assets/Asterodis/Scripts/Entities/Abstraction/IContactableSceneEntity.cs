using System;
using Services.EntityService;

namespace Asterodis.Entities
{
    public interface IContactableSceneEntity : ISceneEntity
    {
        event Action<IContactableSceneEntity, IContactableSceneEntity> OnContact;
        bool IsContactable { get; }
        void SetActiveContacts(bool value);
    }
}