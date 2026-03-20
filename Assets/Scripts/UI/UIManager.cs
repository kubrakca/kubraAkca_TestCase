using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using UnityEngine;
using Zenject;

namespace UI
{
    public class UIManager : MonoBehaviour, IUIService
    {
        [SerializeField] private UIScreenData screenData; 
        
        private readonly Dictionary<Type, UIView> _activeViews = new();
        [Inject] private DiContainer _container;

        public T Show<T>() where T : UIView
        {
            var type = typeof(T);

            if (_activeViews.TryGetValue(type, out var activeView))
            {
                activeView.Show();
                return (T)activeView;
            }

            var prefab = screenData.uıViews.FirstOrDefault(x => x is T);
            
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

        public void Hide<T>() where T : UIView
        {
            if (_activeViews.TryGetValue(typeof(T), out var view))
            {
                view.Hide();
            }
        }

        public void HideAll()
        {
            foreach (var view in _activeViews.Values)
            {
                view.Hide();
            }
        }
    }
}