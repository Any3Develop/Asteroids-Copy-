using System;
using System.Collections.Generic;
using System.Linq;
using Asterodis.Audios;
using Services.EntityService;
using Services.LoggerService;

namespace Asterodis.GameBuilder
{
    public class GameContext : IGameContext, IDisposable
    {
        private readonly List<ITask> tasks;
        
        public string PlayerId { get; set; }
        public bool GameEnd { get; private set; }
        public int Level { get; private set; }
        public int Score { get; private set; }
        public int TaskCount => tasks.Count;
        public float TasksProgress => tasks.Sum(x => x.Progress) / TaskCount;
        public IEnumerable<ITask> Tasks => tasks;
        
        public event Action OnGameEnd;
        public event Action OnLevelChanged;
        public event Action OnScoreChanged;
        public event Action OnTasksCompleted;
        public event Action<IEntity> OnSpawn;
        public event Action<IEntity, IEntity> OnDestoryed;
        public event Action<IEntity, AudioAction> OnAudioReqested;

        public GameContext()
        {
            tasks = new List<ITask>();
        }

        public void ReqestAudio(IEntity entity, AudioAction action)
        {
            OnAudioReqested?.Invoke(entity, action);
        }

        public void Destoyed(IEntity target, IEntity killer)
        {
            OnDestoryed?.Invoke(target, killer);
        }

        public void Spawned(IEntity entity)
        {
            OnSpawn?.Invoke(entity);
        }

        public void AddTask(ITask value)
        {
            if (value == null)
            {
                DefaultLogger.Error("Cant add null task");
                return;
            }

            tasks.Add(value);
        }

        public void CompleteTask(ITask value)
        {
            if (TaskCount <=0)
                return;
            
            tasks.Remove(value);
            value?.Dispose();
            
            if (TaskCount <= 0)
                OnTasksCompleted?.Invoke();
        }

        public void AddScore(int value)
        {
            if (value == 0)
                return;
            
            Score += value;
            OnScoreChanged?.Invoke();
        }

        public void LevelUp(int? value = null)
        {
            if (TaskCount > 0)
            {
                DefaultLogger.Error("Cant change level while tasks not completed");
                return;
            }

            Level += value ?? 1;
            OnLevelChanged?.Invoke();
        }

        public void SetGameEnd(bool value)
        {
            GameEnd = value;
            if (value)
                OnGameEnd?.Invoke();
        }

        public void Reset()
        {
            tasks.ForEach(x => x?.Dispose());
            tasks.Clear();
            OnLevelChanged = null;
            OnScoreChanged = null;
            OnTasksCompleted = null;
            OnAudioReqested = null;
            OnDestoryed = null;
            OnGameEnd = null;
            OnSpawn = null;
            GameEnd = false;
            Level = Score = 0;
            PlayerId = string.Empty;
        }

        public void Dispose()
        {
            Reset();
        }
    }
}