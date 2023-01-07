using System;
using Services.EntityService;
using UnityEngine;

namespace Asterodis.Entities.Weapons
{
    public interface IWeaponSceneEntity : ISceneEntity, IDisposable
    {
        Transform Aim { get; }
    }
}