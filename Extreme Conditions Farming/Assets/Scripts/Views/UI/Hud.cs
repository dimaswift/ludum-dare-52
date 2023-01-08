using System;
using ECF.Domain;
using ECF.Domain.Game;
using TMPro;
using UnityEngine;

namespace ECF.Views.UI
{
    public class Hud : MonoBehaviour
    {
        [SerializeField] private ToolButton toolButton;
        [SerializeField] private TextMeshProUGUI tickText;
        [SerializeField] private TextMeshProUGUI coinsText;
        private void Start()
        {
            toolButton.gameObject.SetActive(false);
            Game.Instance.Phase.Changed += OnPhaseChanged;
            Game.Instance.OnNewSimulationCreated += OnNewSimulationCreated;
            gameObject.SetActive(false);
            foreach (Tool tool in Game.Instance.Tools.Tools)
            {
                var btn = Instantiate(toolButton.gameObject).GetComponent<ToolButton>();
                btn.transform.SetParent(toolButton.transform.parent);
                btn.SetUp(tool);
                btn.gameObject.SetActive(true);
                btn.transform.localScale = Vector3.one;
            }
        }

        private void OnNewSimulationCreated()
        {
            Game.Instance.Simulation.Time.Changed += OnTimeChanged;
            var coins = Game.Instance.Simulation.Inventory.Get(InventoryItems.Coins);
            coins.Changed += OnCoinsChanged;
            OnCoinsChanged(coins.Value);
        }

        private void OnCoinsChanged(int coins)
        {
            coinsText.text = coins.ToString();
        }
        
        private void OnPhaseChanged(GamePhase phase)
        {
            gameObject.SetActive(phase == GamePhase.Playing);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Game.Instance.Pause();
            }
        }

        private string GetDuration(int ticks)
        {
            var span = TimeSpan.FromHours(ticks);
            if (span.Days > 0)
                return $"{span.Days}d {span.Hours}h";
            return $"{span.Hours}h";
        }

        private void OnTimeChanged(int time)
        {
            tickText.text = GetDuration(time);
        }
    }
}