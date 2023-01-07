using System;
using System.Collections.Generic;
using System.Linq;
using Services.Extensions;

namespace Services.PortalService
{
    public class PortalService : IPortalService, IDisposable
    {
        private readonly Dictionary<IPortal, IPortal> linkedPortals;

        public PortalService()
        {
            linkedPortals = new Dictionary<IPortal, IPortal>();
        }

        public void LinkOneWay(IPortal portalA, IPortal portalB)
        {
            if (portalA == null)
                throw new NullReferenceException("Portal A is missing.");

            if (portalB == null)
                throw new NullReferenceException("Portal B is missing.");

            if (portalA == portalB)
                throw new InvalidOperationException("Portals must be different instances.");

            UnLink(portalA);
            portalA.OnContact += HandleTeleport;
            linkedPortals.Add(portalA, portalB);
        }

        public void LinkTwoWay(IPortal portalA, IPortal portalB)
        {
            LinkOneWay(portalA, portalB);
            LinkOneWay(portalB, portalA);
        }

        public void UnLink(params IPortal[] portals)
        {
            if (portals.IsNullOrEmpty() || linkedPortals.IsNullOrEmpty())
                return;

            foreach (var portal in portals)
            {
                if (!linkedPortals.ContainsKey(portal))
                    continue;

                portal.OnContact -= HandleTeleport;
                linkedPortals.Remove(portal);
            }
        }

        private void HandleTeleport(ITeleportable teleportable, IPortal portal)
        {
            if (portal == null)
                return;
            
            if (!linkedPortals.ContainsKey(portal))
                return;

            var linkedPortal = linkedPortals[portal];
            var exitPoint = portal.Evaluate(linkedPortal, teleportable.Origin);
            teleportable.Teleport(exitPoint);
        }

        public void Dispose()
        {
            var removePortals = linkedPortals
                .ToArray()
                .SelectMany(x => new[] {x.Key, x.Value})
                .ToArray();
            UnLink(removePortals);
            linkedPortals.Clear();
        }
    }
}