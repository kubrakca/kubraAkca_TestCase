using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameObject gameStateRootPrefab;

        public override void InstallBindings()
        {
            Container.InstantiatePrefab(gameStateRootPrefab);
        }
    }
}
