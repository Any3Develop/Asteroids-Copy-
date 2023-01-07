using System;
using System.Collections.Generic;
using Asterodis.Settings;
using Services.EntityService.Factory;
using Services.Extensions;
using Services.LoggerService;
using Services.PortalService;
using Services.SettingsService;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Asterodis.Entities.Portals
{
    public class EdgePortal : IEdgePortalEntity, IInitializable, ILateTickable, IDisposable
    {
        private readonly ISceneEntityFactory<IEdgePortalSceneEntity> portalFactory;
        private readonly ISettingsRepository settingsRepository;
        private readonly TickableManager tickableManager;
        private readonly List<ITeleportable> inBounds;

        private IEdgePortalSceneEntity view;
        private ScreenPortalsSetting setting;
        private Plane plane;
        private Edge edge;

        public string Id { get; private set; }
        public event Action<ITeleportable, IPortal> OnContact;

        public EdgePortal(
            string id,
            TickableManager tickableManager,
            ISettingsRepository settingsRepository,
            ISceneEntityFactory<IEdgePortalSceneEntity> portalFactory)
        {
            Id = id;
            this.settingsRepository = settingsRepository;
            this.portalFactory = portalFactory;
            this.tickableManager = tickableManager;
            inBounds = new List<ITeleportable>();
        }

        public void Initialize()
        {
            setting = settingsRepository.Get<ScreenPortalsSetting>();
            var edgePortalView = portalFactory.Create<EdgePortalView>();
            edgePortalView.SetOwnerId(Id);
            edgePortalView.name = Id + "_Portal";
            view = edgePortalView;
            view.OnExit += OnPortalExit;
            view.OnEnter += OnPortalEnter;
            tickableManager.AddLate(this);
        }

        public void Dispose()
        {
            Id = string.Empty;
            OnContact = null;
            tickableManager.RemoveLate(this);
            Object.Destroy(view.Container.gameObject);
            setting = null;
            view = null;
        }

        public void LateTick()
        {
            if (inBounds.IsNullOrEmpty())
                return;

            var inThisFrame = inBounds.ToArray();
            foreach (var teleportable in inThisFrame)
            {
                if (IsBehind(teleportable.Origin))
                    OnContact?.Invoke(teleportable, this);
            }
        }

        public Vector3 Evaluate(IPortal linked, Vector3 point)
        {
            return linked is IEdgePortalEntity linkedEdgePortal
                ? linkedEdgePortal.PointOnEge(GetEdgeT(point))
                : point;
        }

        public void SetEdge(Edge value)
        {
            edge = value;
            view.SetEdge(value);
            plane = new Plane(value.Normal, value.Origin);
        }

        public Vector3 PointOnEge(float t)
        {
            return Vector3.Lerp(edge.PointA, edge.PointB, t) + edge.Normal * setting.ErrorOffset;
        }

        private bool IsBehind(Vector3 point)
        {
            return !plane.GetSide(point);
        }

        private float GetEdgeT(Vector3 point)
        {
            var ab = edge.PointA - edge.PointB;
            var av = point - edge.PointA;
            return (Mathf.Abs(Vector3.Dot(av, ab) / Vector3.Dot(ab, ab)));
        }

        private void OnPortalExit(ITeleportable teleportable)
        {
            inBounds.Remove(teleportable);
        }

        private void OnPortalEnter(ITeleportable teleportable)
        {
            if (teleportable == null || inBounds.Contains(teleportable))
                return;
            inBounds.Add(teleportable);
        }
    }
}