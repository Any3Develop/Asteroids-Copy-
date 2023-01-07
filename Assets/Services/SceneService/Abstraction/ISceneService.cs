using System.Threading.Tasks;

namespace Services.SceneService
{
    public interface ISceneService
    {
        Task LoadSceneAsync(string sceneName);
        Task LoadSceneAsync(int sceneIndex);
    }
}