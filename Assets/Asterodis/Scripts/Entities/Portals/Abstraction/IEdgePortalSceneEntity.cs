using System;
using Services.EntityService;
using Services.PortalService;
using UnityEngine;

namespace Asterodis.Entities.Portals
{
    public interface IEdgePortalSceneEntity : ISceneEntity
    {
        event Action<ITeleportable> OnEnter;
        
        event Action<ITeleportable> OnExit;

        void SetEdge(Edge value);
    }
}