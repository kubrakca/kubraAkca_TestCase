using Core;
using ScriptableObjects;
using UI;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class LevelInstaller : MonoInstaller
    {
        [SerializeField] private LevelDatabase levelDatabase;

        public override void InstallBindings()
        {
            Container.BindInstance(levelDatabase).AsSingle();

            Container.BindInterfacesAndSelfTo<LevelManager>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();

            Container.Bind<LevelSpawner>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();
        }
    }
}
