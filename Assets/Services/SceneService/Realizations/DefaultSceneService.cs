using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Services.SceneService
{
    public class DefaultSceneService : ISceneService
    {
        public async Task LoadSceneAsync(string sceneName)
        {
            await HandleOperationAsync(SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single));
        }

        public async Task LoadSceneAsync(int sceneIndex)
        {
            await HandleOperationAsync(SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single));
        }

        private async Task HandleOperationAsync(AsyncOperation operation)
        {
            if (operation == null)
                return;
            
            while (!operation.isDone && Application.isPlaying)
            {
                await Task.Yield();
            }
        }
    }
}