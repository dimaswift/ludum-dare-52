using ECF.Domain.Game;
using UnityEngine;
using UnityEngine.UI;

namespace ECF.Views.UI
{
    public class GameOverScreen : Window
    {
        [SerializeField] private Button retryButton;

        protected override void OnInit()
        {
            base.OnInit();
            retryButton.onClick.AddListener(Retry);
        }

        private async void Retry()
        {
            Game.Instance.StartNew();
            await Hide();
        }
    }
}