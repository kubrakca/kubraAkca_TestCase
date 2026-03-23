using ScriptableObjects;
using UI;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class LevelInstaller : MonoInstaller
    {
        #region SerializeField

        [SerializeField] private LevelDatabase levelDatabase;

        #endregion

        #region Public Methods

        public override void InstallBindings()
        {
            Container.BindInstance(levelDatabase).AsSingle();

            Container.BindInterfacesAndSelfTo<LevelManager>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();
        }

        #endregion
    }
}
