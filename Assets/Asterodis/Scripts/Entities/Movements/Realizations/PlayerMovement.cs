using System;
using System.Collections.Generic;
using Asterodis.Entities.Statistics;
using Asterodis.Input;
using Asterodis.Settings;
using Services.AbstractFactoryService;
using Services.EntityService;
using Services.EntityService.Storage;
using Services.Extensions;
using Services.InputService;
using Services.SettingsService;
using UnityEngine;
using Zenject;

namespace Asterodis.Entities.Movements
{
    public class PlayerMovement : IMovement, IInitializable
    {
        private readonly Transform target;
        private readonly IInputController<MovementActions> inputController;
        private readonly IEntityStorage<IStatisticEntity> statisticStorage;
        private readonly ISettingsRepository settingsRepository;
        private readonly IAbstractFactory abstractFactory;
        private readonly List<IStatisticEntity> statisticEntities;
        private bool hasAcellecration;
        private bool hasRotation;
        private float rotationDir;
        private float rotationAngle;
        private PlayerMovementSetting setting;
        private Vector3 velocity;
        
        public event Action OnMove;
        public string Id { get; }
        
        public PlayerMovement(
            string id,
            Transform target,
            IInputController<MovementActions> inputController,
            IEntityStorage<IStatisticEntity> statisticStorage,
            ISettingsRepository settingsRepository,
            IAbstractFactory abstractFactory)
        {
            Id = id;
            this.target = target;
            this.inputController = inputController;
            this.statisticStorage = statisticStorage;
            this.settingsRepository = settingsRepository;
            this.abstractFactory = abstractFactory;
            statisticEntities = new List<IStatisticEntity>();
        }

        public void Initialize()
        {
            setting = settingsRepository.Get<PlayerMovementSetting>(nameof(PlayerMovement));
            inputController.GetAll().ForEach(x=> x.OnAnyStateChanged += OnInputTriggered);
            
            var coordStatistic = abstractFactory.Create<StatisticEntity>(Id);
            var angleStatistic = abstractFactory.Create<StatisticEntity>(Id);
            var speedStatistic = abstractFactory.Create<StatisticEntity>(Id);
            statisticEntities.AddRange(new [] {coordStatistic, angleStatistic, speedStatistic});

            coordStatistic.SetIndex(0);
            angleStatistic.SetIndex(1);
            speedStatistic.SetIndex(2);
            coordStatistic.OnRefreshed += () =>
            {
                var pos = target.position * 100f; // where 100 to display correct
                coordStatistic.SetTitle("Coords");
                coordStatistic.SetValue($"[x:{Mathf.FloorToInt(pos.x)}]<br>[y:{Mathf.FloorToInt(pos.y)}]");
            };
            
            angleStatistic.OnRefreshed += () =>
            {
                angleStatistic.SetTitle("Angle");
                angleStatistic.SetValue($"[{Mathf.RoundToInt(rotationAngle)}]");
            };
            
            speedStatistic.OnRefreshed += () =>
            {
                speedStatistic.SetTitle("Speed");
                speedStatistic.SetValue($"[{Mathf.RoundToInt(velocity.Abs().Max() * 10000)}]"); // where 10000 to display correct
            };
            
            statisticStorage.Add(statisticEntities);
        }

        public void Dispose()
        {
            statisticEntities.ForEach(x =>
            {
                statisticStorage.Remove(x);
                x?.Dispose();
            });
            statisticEntities.Clear();
            inputController.GetAll().ForEach(x => x.OnAnyStateChanged -= OnInputTriggered);
            OnMove = null;
        }

        public void SetPosition(Vector3 value)
        {
            target.position = value;
        }

        public void Update()
        {
            if (target == null)
                return;
            
            if (hasAcellecration)
            {
                var max = setting.AccelerationMovementMax;
                var min = -setting.AccelerationMovementMax;
                velocity += target.up * setting.AccelerationMovementFactor * Time.deltaTime;
                velocity = velocity.Clamp(min, max).SetZ(target.position.z);
                OnMove?.Invoke();
            }
            else
            {
                var decelerationT = setting.DecelerationMovementFactor * Time.deltaTime;
                velocity = Vector3.Lerp(velocity, Vector3.zero.SetZ(velocity.z), decelerationT);
            }

            if (hasRotation)
            {
                rotationAngle += rotationDir * setting.AccelerationRotationFactor * Time.deltaTime;
                FixRotation();
            }
            else
            {
                var decelerationT = setting.DecelerationRotationFactor * Time.deltaTime;
                rotationDir = Mathf.Lerp(rotationDir, 0, decelerationT);
                rotationAngle += rotationDir * setting.AccelerationRotationFactor * Time.deltaTime;
                FixRotation();
            }
            
            target.rotation = Quaternion.AngleAxis(rotationAngle, Vector3.forward);
            target.position += velocity;
        }

        private void FixRotation()
        {
            var signedDir = Mathf.Sign(rotationDir);
            var modulatedAngle = Mathf.Abs(rotationAngle);
            rotationAngle = modulatedAngle >= 360f ? (360f - modulatedAngle) * signedDir : rotationAngle;
        }

        private void OnInputTriggered(IInputContext context)
        {
            switch (context.Id)
            {
                case nameof(MovementActions.Acceleration):
                    hasAcellecration = context.Performed;
                    return;
                
                case nameof(MovementActions.Rotation):
                    hasRotation = context.Performed;
                    if (hasRotation)
                        rotationDir = context.ReadValue<float>();
                    break;
            }
        }
    }
}