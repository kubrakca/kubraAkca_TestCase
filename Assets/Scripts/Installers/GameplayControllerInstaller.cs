using Core;
using Zenject;

namespace Installers
{
    public class GameplayControllerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameplayController>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();
        }
    }
}
