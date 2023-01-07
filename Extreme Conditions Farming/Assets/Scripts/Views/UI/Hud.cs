using System;
using ECF.Domain.Game;
using TMPro;
using UnityEngine;

namespace ECF.Views.UI
{
    public class Hud : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tickText;
        
        private void Start()
        {
            Game.Instance.Phase.Changed += OnPhaseChanged;
            Game.Instance.OnNewSimulationCreated += OnNewSimulationCreated;
            gameObject.SetActive(false);
        }

        private void OnNewSimulationCreated()
        {
            Game.Instance.Simulation.Time.Changed += OnTimeChanged;
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

        private void OnTimeChanged(int time)
        {
            tickText.text = time.ToString();
        }
    }
}