using Core;
using Core.CameraRig;
using Zenject;

namespace Installers
{
    public class LevelSpawnerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IGameplayCamera>()
                .To<GameplayCameraProvider>()
                .AsSingle();

            Container.Bind<LevelSpawner>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();
        }
    }
}
