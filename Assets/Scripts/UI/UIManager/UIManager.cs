using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using UnityEngine;
using Zenject;

namespace UI
{
    /// <summary>Resolves UI prefabs from <see cref="UIScreenData"/> via Zenject and keeps one live instance per screen type.</summary>
    public class UIManager : MonoBehaviour, IUIService
    {
        #region SerializeField

        [SerializeField] private UIScreenData screenData;

        #endregion

        #region Private Fields

        private readonly Dictionary<Type, UIView> _activeViews = new();

        #endregion

        #region Dependency Injection

        [Inject] private DiContainer _container;

        #endregion

        #region Public Methods

        /// <summary>Shows existing instance or instantiates prefab matching <typeparamref name="T"/>.</summary>
        public T Show<T>() where T : UIView
        {
            var type = typeof(T);

            if (_activeViews.TryGetValue(type, out var activeView))
            {
                activeView.Show();
                return (T)activeView;
            }

            var prefab = screenData.uiViews.FirstOrDefault(x => x is T);

            if (prefab == null)
            {
                Debug.LogError($"{type.Name}  {screenData.name} can not found!");
                return null;
            }

            var newView = _container.InstantiatePrefabForComponent<T>(prefab, transform);
            _activeViews.Add(type, newView);
            newView.Show();

            return newView;
        }

        /// <summary>Hides the active view of this type without destroying it.</summary>
        public void Hide<T>() where T : UIView
        {
            if (_activeViews.TryGetValue(typeof(T), out var view))
            {
                view.Hide();
            }
        }

        /// <summary>Calls <see cref="UIView.Hide"/> on every cached screen.</summary>
        public void HideAll()
        {
            foreach (var view in _activeViews.Values)
            {
                view.Hide();
            }
        }

        #endregion
    }
}
