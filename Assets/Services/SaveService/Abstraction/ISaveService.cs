namespace Services.SaveService
{
    public interface ISaveService
    {
        void Save(string path, string data);
        string Load(string path);
    }
}