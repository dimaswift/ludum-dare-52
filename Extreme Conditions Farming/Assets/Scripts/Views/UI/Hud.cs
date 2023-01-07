using System;
using ECF.Domain.Game;
using TMPro;
using UnityEngine;

namespace ECF.Views.UI
{
    public class Hud : MonoBehaviour
    {
        [SerializeField] private ToolButton toolButton;
        [SerializeField] private TextMeshProUGUI tickText;
     
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