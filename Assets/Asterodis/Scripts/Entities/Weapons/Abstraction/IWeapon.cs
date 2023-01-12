using System;
using Services.EntityService;

namespace Asterodis.Entities.Weapons
{
    public interface IWeapon : IEntity, IDisposable
    {
        void Initialize(string ownerId, params IWeaponSceneEntity[] weaponeViews);
        void Release();
    }
}