using System;
using System.Collections.Generic;
using Asterodis.Audios;
using Services.EntityService;

namespace Asterodis.GameBuilder
{
    public interface IGameContext
    {
        string PlayerId { get; set; }
        bool GameEnd { get; }
        int Level { get; }
        int Score { get; }
        int TaskCount { get; }
        float TasksProgress { get; }
        IEnumerable<ITask> Tasks { get; }
        
        event Action OnGameEnd;
        event Action OnLevelChanged;
        event Action OnScoreChanged;
        event Action OnTasksCompleted;
        event Action<IEntity> OnSpawn;
        event Action<IEntity, IEntity> OnDestoryed;
        event Action<IEntity, AudioAction> OnAudioReqested;
        
        void ReqestAudio(IEntity entity, AudioAction action);
        void Destoyed(IEntity target, IEntity killer);
        void Spawned(IEntity entity);
        void AddTask(ITask value);
        void CompleteTask(ITask value);
        void LevelUp(int? value = null);
        void AddScore(int value);
        void SetGameEnd(bool value);
        void Reset();
    }
}