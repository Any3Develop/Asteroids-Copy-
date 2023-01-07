using UnityEngine;
using Zenject;

namespace Services.CameraService
{
    public class CameraServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<Camera>()
                .FromComponentInNewPrefabResource("Cameras/GameCamera")
                .AsSingle()
                .NonLazy();
        }
    }
}