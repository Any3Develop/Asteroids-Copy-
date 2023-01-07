using System;
using Services.EntityService;
using UnityEngine;

namespace Asterodis.Entities.Movements
{
    public interface IMovement : IDisposable, IEntity
    {
        void SetPosition(Vector3 value);
        void Update();
    }
}