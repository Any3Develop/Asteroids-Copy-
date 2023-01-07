using UnityEngine;

namespace Services.PortalService
{
    public interface ITeleportable
    {
        Vector3 Origin { get; }
        void Teleport(Vector3 point);
    }
}