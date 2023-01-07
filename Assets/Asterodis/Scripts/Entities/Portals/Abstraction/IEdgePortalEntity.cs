using Services.EntityService;
using Services.PortalService;
using UnityEngine;

namespace Asterodis.Entities.Portals
{
    public interface IEdgePortalEntity : IPortal, IEntity
    {
        Vector3 PointOnEge(float t);
        void SetEdge(Edge value);
    }
}