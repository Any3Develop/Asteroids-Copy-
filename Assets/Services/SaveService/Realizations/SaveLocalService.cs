using UnityEngine;

namespace Services.SaveService
{
    public class SaveLocalService : ISaveService
    {
        public void Save(string path, string data)
        {
            PlayerPrefs.SetString(path, data);
        }

        public string Load(string path)
        {
            var loaded = PlayerPrefs.GetString(path);
            return string.IsNullOrEmpty(loaded)
                ? Resources.Load<TextAsset>(path)?.text ?? string.Empty
                : loaded;
        }
    }
}