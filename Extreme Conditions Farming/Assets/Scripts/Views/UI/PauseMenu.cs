using UnityEngine;
using UnityEngine.UI;

namespace ECF.Views.UI
{
    public class PauseMenu : Window
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private Button mainMenu;

        protected override void OnInit()
        {
            base.OnInit();
            continueButton.onClick.AddListener(Continue);
            mainMenu.onClick.AddListener(OnMainMenu);
        }

        private async void Continue()
        {
            Game.Instance.Resume();
            await Hide();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Game.Instance.Resume();
                Continue();
            }
        }
        
        private async void OnMainMenu()
        {
            Game.Instance.ToMainMenu();
            await Hide();
        }
    }
}