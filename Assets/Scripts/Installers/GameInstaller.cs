using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        #region SerializeField

        [SerializeField] private GameObject gameStateRootPrefab;

        #endregion

        #region Public Methods

        public override void InstallBindings()
        {
            Container.InstantiatePrefab(gameStateRootPrefab);
        }

        #endregion
    }
}
