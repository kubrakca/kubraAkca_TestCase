using System.Collections.Generic;
using UI;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "UIScreenData", menuName = "UI/New Screen Data")]
    
    public class UIScreenData : ScriptableObject
    {
        public List<UIView> uiViews;
    }
}