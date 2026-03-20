using UI;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class UIInstaller : MonoInstaller
    {
        [SerializeField] private UIManager uiManagerPrefab;

        // ReSharper disable Unity.PerformanceAnalysis
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UIManager>()
                .FromComponentInNewPrefab(uiManagerPrefab)
                .AsSingle()
                .NonLazy();
        }
    }
}