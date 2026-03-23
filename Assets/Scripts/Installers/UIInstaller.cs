using UI;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class UIInstaller : MonoInstaller
    {
        #region SerializeField

        [SerializeField] private UIManager uiManagerPrefab;

        #endregion

        #region Public Methods

        // ReSharper disable Unity.PerformanceAnalysis
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UIManager>()
                .FromComponentInNewPrefab(uiManagerPrefab)
                .AsSingle()
                .NonLazy();
        }

        #endregion
    }
}
