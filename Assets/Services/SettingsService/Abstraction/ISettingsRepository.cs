namespace Services.SettingsService
{
    public interface ISettingsRepository
    {
        T Get<T>(string name);
        
        T Get<T>();

        void Add(object setting, string name);
        
        void Add(object setting);

        void Remove(string name);

        void Clear();

        void Save();
    }
}