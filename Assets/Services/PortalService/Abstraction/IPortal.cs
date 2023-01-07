using System;
using UnityEngine;

namespace Services.PortalService
{
    public interface IPortal
    {
        event Action<ITeleportable, IPortal> OnContact;
        public Vector3 Evaluate(IPortal linked, Vector3 point);
    }
}