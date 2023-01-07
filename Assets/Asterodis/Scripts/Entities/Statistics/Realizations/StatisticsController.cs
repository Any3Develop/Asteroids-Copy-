using System;
using System.Collections.Generic;
using System.Linq;
using Asterodis.GameBuilder;
using Asterodis.UIWindows;
using Services.EntityService.Factory;
using Services.EntityService.Storage;
using Services.Extensions;
using Services.ImageLoaderService;
using Services.UIService;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Asterodis.Entities.Statistics
{
    public class StatisticsController : IInitializable, ITickable, IDisposable
    {
        private readonly IUIService uiService;
        private readonly IGameContext gameContext;
        private readonly IImageLoader imageLoader;
        private readonly ISceneEntityFactory<IStatisticSceneEntity> statisticFactory;
        private readonly IEntityStorage<IStatisticEntity> statisticsStorage;
        private readonly TickableManager tickableManager;
        private readonly Dictionary<IStatisticEntity, IStatisticSceneEntity> stataisticEntitites;
        private Transform container;
        private bool disposed;

        public StatisticsController(
            IUIService uiService,
            IGameContext gameContext,
            IImageLoader imageLoader,
            ISceneEntityFactory<IStatisticSceneEntity> statisticFactory,
            IEntityStorage<IStatisticEntity> statisticsStorage,
            TickableManager tickableManager)
        {
            this.uiService = uiService;
            this.gameContext = gameContext;
            this.imageLoader = imageLoader;
            this.statisticFactory = statisticFactory;
            this.statisticsStorage = statisticsStorage;
            this.tickableManager = tickableManager;
            stataisticEntitites = new Dictionary<IStatisticEntity, IStatisticSceneEntity>();
        }

        public void Initialize()
        {
            gameContext.OnScoreChanged += OnScoreChanged;
            gameContext.OnLevelChanged += OnLevelChanged;
            gameContext.OnGameEnd += Dispose;
            statisticsStorage.OnAdded += OnStatisticsAdded;
            statisticsStorage.OnRemoved += OnStatisticRemoved;
            tickableManager.Add(this);
            uiService.Show<UIGameStats>();
            var window = uiService.Show<UIStatistics>();
            container = window.ContentContainer;
            OnLevelChanged();
            OnScoreChanged();
        }

        public void Dispose()
        {
            if (disposed)
                return;
            
            disposed = true;
            gameContext.OnScoreChanged -= OnScoreChanged;
            gameContext.OnLevelChanged -= OnLevelChanged;
            statisticsStorage.OnAdded -= OnStatisticsAdded;
            statisticsStorage.OnRemoved -= OnStatisticRemoved;
            gameContext.OnGameEnd -= Dispose;
            uiService.Hide<UIStatistics>();
            uiService.Hide<UIGameStats>();
            stataisticEntitites.Clear();
            container.DestroyChilds();
            container = null;
            tickableManager.Remove(this);
        }
        
        public void Tick()
        {
            if (gameContext.GameEnd || string.IsNullOrEmpty(gameContext.PlayerId))
                return;

            foreach (var (key, value) in stataisticEntitites.ToArray())
            {
                Refresh(key, value);
            }
        }

        private void Refresh(IStatisticEntity entity, IStatisticSceneEntity view)
        {
            if (view == null || entity == null)
                    return;
                
            entity.Refresh(); // before read to collect fresh info
            view.SetText(entity.Value);
            view.SetIndex(entity.Index);
        }

        private void Initialize(IStatisticEntity entity, IStatisticSceneEntity view)
        {
            if (view == null)
                return;
            
            view.SetId(entity.Id);
            var iconsPath = statisticFactory.ResourcePath.Replace("Prefabs", "Icons/");
            view.SetActiveIcon(imageLoader.TryLoadSpite(iconsPath + entity.Title, out var icon));
            view.SetIcon(icon);
            view.SetTitle(entity.Title);
        }

        private void OnLevelChanged()
        {
            uiService.Get<UIGameStats>().SetLevelText($"Level<br>{gameContext.Level}");
        }

        private void OnScoreChanged()
        {
            uiService.Get<UIGameStats>().SetScoreText($"Score<br>{gameContext.Score}");
        }

        private void OnStatisticsAdded(IStatisticEntity entity)
        {
            if (gameContext.PlayerId != entity.Id || stataisticEntitites.ContainsKey(entity))
                return;

            var view = statisticFactory.Create<StatisticView>(parent:container);
            stataisticEntitites.Add(entity, view);
            Refresh(entity, view);
            Initialize(entity, view);
        }
        
        private void OnStatisticRemoved(IStatisticEntity entity)
        {
            if (gameContext.PlayerId != entity.Id || !stataisticEntitites.ContainsKey(entity))
                return;

            var view = stataisticEntitites[entity];
            stataisticEntitites.Remove(entity);
            Object.Destroy(view.Container.gameObject);
        }
    }
}