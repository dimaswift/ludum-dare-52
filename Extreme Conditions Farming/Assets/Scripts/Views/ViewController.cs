using System;
using System.Collections.Generic;
using UnityEngine;

namespace ECF.Views
{
    public class ViewController : MonoBehaviour
    {
        [SerializeField] private SimulationView simulationView;
        
        public SimulationView SimulationView { get; private set; }
        
        private void Start()
        {
            Game.Instance.OnNewSimulationCreated += OnNewSimulationCreated;
            simulationView.gameObject.SetActive(false);
        }

        private void OnNewSimulationCreated()
        {
            if (SimulationView != null)
            {
                SimulationView.Dispose();
            }

            SimulationView = Instantiate(simulationView.gameObject).GetComponent<SimulationView>();
            SimulationView.transform.SetPositionAndRotation(simulationView.transform.position, simulationView.transform.rotation);
            SimulationView.transform.SetParent(transform);
            SimulationView.gameObject.SetActive(true);
            SimulationView.Init(Game.Instance.Simulation);
        }
    }
}