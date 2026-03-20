using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StartScreen : UIView
    {
        [SerializeField] private Button startButton;

        public void OnStartButtonClicked()
        {
           
        }

        public override void Show()
        {
            base.Show();
            Debug.Log("Start Screen.");
        }

        public override void Hide()
        {
            base.Hide();
            Debug.Log("Start Screen.");
        }
    }
}