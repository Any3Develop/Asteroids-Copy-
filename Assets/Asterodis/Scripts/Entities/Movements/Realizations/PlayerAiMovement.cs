using System;
using System.Linq;
using Asterodis.Entities.Players;
using Asterodis.GameBuilder;
using Asterodis.Settings;
using Services.EntityService.Storage;
using Services.Extensions;
using Services.SettingsService;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Asterodis.Entities.Movements
{
    public class PlayerAiMovement : IMovement, IInitializable
    {
        private readonly int variation;
        private readonly Transform movementTarget;
        private readonly IGameContext gameContext;
        private readonly IEntityStorage<IAiTargetSceneEntity> targetsStorage;
        private readonly ISettingsRepository settingsRepository;
        
        private PlayerAiMovementVariation variationSetting;
        private PlayerAiMovementSetting movementSetting;
        private IAiTargetSceneEntity target;
        private Vector3 lastTargetPosition;
        private bool playerIsInteresting;
        private int maneuverLeft;
        
        public string Id { get; }
        public event Action OnManeuversCompleted;
        public event Action OnMove;

        public PlayerAiMovement(
            string id,
            int variation,
            Transform movementTarget,
            IGameContext gameContext,
            IEntityStorage<IAiTargetSceneEntity> targetsStorage,
            ISettingsRepository settingsRepository)
        {
            Id = id;
            this.variation = variation;
            this.movementTarget = movementTarget;
            this.gameContext = gameContext;
            this.targetsStorage = targetsStorage;
            this.settingsRepository = settingsRepository;
        }

        public void Initialize()
        {
            playerIsInteresting = true;
            movementSetting = settingsRepository.Get<PlayerAiMovementSetting>();
            variationSetting = movementSetting.Variations[variation];
            targetsStorage.OnAdded += OnTargetAvailable;
            targetsStorage.OnRemoved += OnTargetRemoved;
            maneuverLeft = Mathf.Abs(Random.Range(movementSetting.ManeuverCountMin, movementSetting.ManeuverCountMax));
            SelectAvailableTarget();
        }

        public void Dispose()
        {
            OnMove = null;
            OnManeuversCompleted = null;
            targetsStorage.OnAdded -= OnTargetAvailable;
            targetsStorage.OnRemoved -= OnTargetRemoved;
            variationSetting = null;
            target = null;
        }

        public void SetPosition(Vector3 value)
        {
            movementTarget.position = value;
        }

        public void Update()
        {
            if (maneuverLeft <= 0)
            {
                OnManeuversCompleted?.Invoke();
                return;
            }

            if (target is not {IsValidTarget: true})
                SelectAvailableTarget();

            if (target == null)
                return;
            
            var direction = target.Container.position - movementTarget.position;
            if (direction.magnitude <= movementSetting.ReachDistance)
            {
                maneuverLeft--;
                target = null;
                return;
            }

            movementTarget.Translate(direction.normalized * (variationSetting.LinearSpeed * Time.deltaTime));
            OnMove?.Invoke();
        }

        private void SelectAvailableTarget()
        {
            if (playerIsInteresting && targetsStorage.Contains(gameContext.PlayerId))
            {
                target = targetsStorage.Get(gameContext.PlayerId);
                if (target.IsValidTarget)
                    return;
            }

            if (target?.Container != null)
                lastTargetPosition = target.Container.position;
            
            target =  targetsStorage.Get().Shuffle().FirstOrDefault(ValidateTarget);

            bool ValidateTarget(IAiTargetSceneEntity aiTarget)
            {
                if (lastTargetPosition == Vector3.zero)
                    return true;
                
                if (aiTarget?.Container == null)
                    return false;
                
                var point = aiTarget.Container.position;
                return aiTarget.IsValidTarget
                       && Mathf.RoundToInt(point.x) != Mathf.RoundToInt(lastTargetPosition.x)
                       && Mathf.RoundToInt(point.y) != Mathf.RoundToInt(lastTargetPosition.y);
            }
        }

        private void OnTargetRemoved(IAiTargetSceneEntity entity)
        {
            if (entity?.Id == gameContext.PlayerId)
                playerIsInteresting = false;
            
            if (target == entity)
                SelectAvailableTarget();
        }

        private void OnTargetAvailable(IAiTargetSceneEntity entity)
        {
            if (entity?.Id == gameContext.PlayerId && playerIsInteresting)
                target = entity;
        }
    }
}