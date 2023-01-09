using System.Threading.Tasks;
using ECF.Domain.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ECF.Views.UI
{
    public class GameOverScreen : Window
    {
        [SerializeField] private TextMeshProUGUI survivedTimeText;

        [SerializeField] private Button retryButton;

        protected override void OnInit()
        {
            base.OnInit();
            retryButton.onClick.AddListener(Retry);
        }

        protected override Task ProcessShow()
        {
            survivedTimeText.text = $"You survived for {Game.Instance.Simulation.Time.Value.GetDurationText()}";
            return base.ProcessShow();
        }


        private async void Retry()
        {
            Game.Instance.StartNew();
            await Hide();
        }
    }
}