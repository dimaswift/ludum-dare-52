using System.Threading.Tasks;
using ECF.Domain.Game;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ECF.Views.UI
{
    public class MainMenu : Window
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button exitButton;

        protected override void OnInit()
        {
            base.OnInit();
            startButton.onClick.AddListener(StartNew);
            continueButton.onClick.AddListener(Continue);
            exitButton.onClick.AddListener(Quit);
        }

        private async void Continue()
        {
            Game.Instance.Continue();
            await Hide();
        }

        private async void StartNew()
        {
            Game.Instance.StartNew();
            await Hide();
        }

        protected override Task ProcessShow()
        {
            continueButton.gameObject.SetActive(Game.Instance.HasSavedSimulation);
            return base.ProcessShow();
        }

        private async void Quit()
        {
            Game.Instance.Quit();
            await Hide();
        }
    }
}
