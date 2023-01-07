using Zenject;

namespace Services.AudioService
{
    public class AudioPlayerFactory : IAudioPlayerFactory
    {
        private readonly string resourcePath;
        private readonly IInstantiator instantiator;

        public AudioPlayerFactory(string resourcePath, IInstantiator instantiator)
        {
            this.resourcePath = resourcePath;
            this.instantiator = instantiator;
        }
        
        public IAudioPlayer Create()
        {
            return instantiator.InstantiatePrefabResourceForComponent<IAudioPlayer>(resourcePath);
        }
    }
}