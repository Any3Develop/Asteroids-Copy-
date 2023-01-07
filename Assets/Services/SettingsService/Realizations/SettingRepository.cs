using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Services.LoggerService;
using Services.SaveService;
using Zenject;

namespace Services.SettingsService
{
    public class Wrapper
    {
        public string Name;
        public string Json;
        public Type Type;
    }

    public class SettingRepository : ISettingsRepository, IInitializable
    {
        private const string FilePath = "Settings/GameSettings";
        private readonly ISaveService saveService;
        private readonly Dictionary<string, object> storage;

        public SettingRepository(ISaveService saveService)
        {
            this.saveService = saveService;
            storage = new Dictionary<string, object>();
        }

        public void Initialize()
        {
            var json = saveService.Load(FilePath);
            if (string.IsNullOrEmpty(json))
            {
                DefaultLogger.Error("Any settings not loaded!");
                return;
            }

            try
            {
                var saveDatas = JsonConvert.DeserializeObject<List<Wrapper>>(json);
                foreach (var wrapper in saveDatas)
                {
                    var setting = JsonConvert.DeserializeObject(wrapper.Json, wrapper.Type);
                    storage.Add(wrapper.Name, setting);
                }
            }
            catch (Exception e)
            {
                DefaultLogger.Error("Cant load settings.");
                DefaultLogger.Error(e);
            }
        }

        public T Get<T>(string name)
        {
            if (!storage.ContainsKey(name))
            {
                DefaultLogger.Error($"Setting with name : {name} does not represent in collection.");
                return default;
            }

            if (storage[name] is T tSetting)
                return tSetting;

            DefaultLogger.Error($"Setting with name : {name} does not implement type : {typeof(T).Name}." +
                                $"Try to request with type : {storage[name]?.GetType().Name}");
            return default;
        }

        public T Get<T>()
        {
            return Get<T>(typeof(T).Name);
        }

        public void Add(object setting, string name)
        {
            if (setting == null)
            {
                DefaultLogger.Error($"Setting with name : '{name ?? "null"}' object setting is missing.");
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                DefaultLogger.Error($"Setting name is empty for target setting : {setting.GetType().Name}");
                return;
            }

            if (storage.ContainsKey(name))
                DefaultLogger.Log($"Setting with name : {name} will be rewritten.");

            storage[name] = setting;
        }

        public void Add(object setting)
        {
            if (setting == null)
            {
                DefaultLogger.Error($"Setting object is missing.");
                return;
            }

            Add(setting, setting.GetType().Name);
        }

        public void Remove(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            storage.Remove(name);
        }

        public void Clear()
        {
            storage.Clear();
        }

        public void Save()
        {
            var wrappers = storage
                .Where(x => x.Value != null && !string.IsNullOrEmpty(x.Key))
                .Select(item => new Wrapper
                {
                    Name = item.Key,
                    Json = JsonConvert.SerializeObject(item.Value),
                    Type = item.Value.GetType()
                }).ToList();

            saveService.Save(FilePath, JsonConvert.SerializeObject(wrappers));
        }
    }
}