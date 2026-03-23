using Core;
using Zenject;

namespace Installers
{
    public class GameplayControllerInstaller : MonoInstaller
    {
        #region Public Methods

        public override void InstallBindings()
        {
            Container.Bind<GameplayController>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();
        }

        #endregion
    }
}
