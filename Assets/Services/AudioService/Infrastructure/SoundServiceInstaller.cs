using Zenject;

namespace Services.AudioService
{
    public class SoundServiceInstaller : Installer<SoundServiceInstaller>
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<AudioService>()
                .AsSingle();

            Container
                .BindInterfacesTo<AudioPlayerFactory>()
                .AsSingle()
                .WithArguments("Audio/AudioPlayerView");

            Container
                .BindInterfacesTo<ClipStorage>()
                .AsSingle()
                .WithArguments("Audio/Clips");
        }
    }
}