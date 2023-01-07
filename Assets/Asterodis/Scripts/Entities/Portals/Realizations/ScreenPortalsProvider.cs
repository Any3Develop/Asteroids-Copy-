using System;
using System.Collections.Generic;
using Asterodis.Settings;
using Services.AbstractFactoryService;
using Services.Extensions;
using Services.LoggerService;
using Services.PortalService;
using Services.SettingsService;
using UnityEngine;
using Zenject;

namespace Asterodis.Entities.Portals
{
    public class ScreenPortalsProvider : IPortalsProvider, IInitializable, ILateTickable
    {
        private readonly Camera gameCamera;
        private readonly ISettingsRepository settingsRepository;
        private readonly IPortalService portalService;
        private readonly IAbstractFactory abstractFactory;
        private readonly List<IEdgePortalEntity> portals;
        private ScreenPortalsSetting setting;
        private GameSettings gameSettings;
        private int screenWidthLast;
        private int screenHeightLast;
        
        public ScreenPortalsProvider(
            Camera gameCamera,
            IAbstractFactory abstractFactory,
            ISettingsRepository settingsRepository,
            IPortalService portalService)
        {
            this.gameCamera = gameCamera;
            this.settingsRepository = settingsRepository;
            this.portalService = portalService;
            this.abstractFactory = abstractFactory;
            portals = new List<IEdgePortalEntity>();
        }

        public void Initialize()
        {
            gameSettings = settingsRepository.Get<GameSettings>();
            setting = settingsRepository.Get<ScreenPortalsSetting>();
            RebuildScreenEdges();
        }


        public IEnumerable<IPortal> GetPortals()
        {
            return portals;
        }

        private void RebuildScreenEdges()
        {
            screenWidthLast = Screen.width;
            screenHeightLast = Screen.height;
            var edges = GetEdges();
            if (edges == null || edges.Length % 2 != 0)
                throw new InvalidOperationException("You cannot bind zero or an odd number of portals");

            if (portals.Count == 0)
                BindPortals(edges.Length);

            for (var i = 0; i < edges.Length; i += 2)
            {
                portals[i].SetEdge(edges[i]);
                portals[i + 1].SetEdge(edges[i + 1]);
            }
        }

        private void BindPortals(int portalCount)
        {
            if (portalCount == 0 || portalCount % 2 != 0)
                throw new InvalidOperationException("You cannot bind zero or an odd number of portals");
            var portalIds = new[] {"Left", "Right", "Top", "Bottom"};
            for (var i = 0; i < portalCount; i += 2)
            {
                var portalPart0 = abstractFactory.Create<EdgePortal>(portalIds[i]);
                var portalPart1 = abstractFactory.Create<EdgePortal>(portalIds[i+1]);

                portalService.LinkTwoWay(portalPart0, portalPart1);

                portals.Add(portalPart0);
                portals.Add(portalPart1);
            }
        }

        private Edge[] GetEdges()
        {
            var edgeRadius = setting.EdgeRadius;
            var sceneDepth = gameSettings.SceneDepth;
            var upperLeft = gameCamera.ScreenToWorldPoint(new Vector3(0, Screen.height)).SetZ(sceneDepth);
            var upperRight = gameCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height)).SetZ(sceneDepth);
            var lowerLeft = gameCamera.ScreenToWorldPoint(Vector3.zero).SetZ(sceneDepth);
            var lowerRight = gameCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0)).SetZ(sceneDepth);

            return new[]
            {
                new Edge(upperLeft, lowerLeft, Vector3.right, edgeRadius), // left
                new Edge(upperRight, lowerRight, Vector3.left, edgeRadius), // right
                new Edge(upperLeft, upperRight, Vector3.down, edgeRadius), // top
                new Edge(lowerLeft, lowerRight, Vector3.up, edgeRadius), // bottom
            };
        }
        
        public void LateTick()
        {
            if (Time.frameCount % 10 != 0) 
                return;
            
            if (screenWidthLast == Screen.width 
                && screenHeightLast == Screen.height) 
                return;

            RebuildScreenEdges();
        }
    }
}