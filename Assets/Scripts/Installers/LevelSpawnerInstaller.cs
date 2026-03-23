using Core;
using Zenject;

namespace Installers
{
    public class LevelSpawnerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<LevelSpawner>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();
        }
    }
}
