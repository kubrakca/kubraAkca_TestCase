using Core;
using Zenject;

namespace Installers
{
    public class LevelSpawnerInstaller : MonoInstaller
    {
        #region Public Methods

        public override void InstallBindings()
        {
            Container.Bind<LevelSpawner>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();
        }

        #endregion
    }
}
