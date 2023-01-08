using System;
using UnityEngine;

namespace ECF.Views
{
    public class ToolController : MonoBehaviour
    {
        public event Action OnToolChanged; 
        public Tool Current => currentTool;
        public Tool[] Tools { get; private set; }
        
        private Tool currentTool;
        
        private float noTargetTime;
        
        private void Awake()
        {
            Game.Instance.OnNewSimulationCreated += OnNewSimulationCreated;
            Tools = GetComponentsInChildren<Tool>();
            foreach (Tool tool in Tools)
            {
                tool.gameObject.SetActive(false);
            }
        }

        private void OnNewSimulationCreated()
        {
            foreach (Tool tool in Tools)
            {
                tool.Init(Game.Instance.Simulation);
            }
        }

        public void SetActiveTool(Tool tool)
        {
            if (currentTool == tool)
            {
                return;
            }
            if (currentTool != null)
            {
                currentTool.gameObject.SetActive(false);
            }
            currentTool = tool;
            currentTool.gameObject.SetActive(true);
            OnToolChanged?.Invoke();
        }

        private void Update()
        {
            if (currentTool == null)
            {
                return;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                if (currentTool.CanActivate(currentTool.CurrentTarget))
                {
                    currentTool.Activate();
                }
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                currentTool.Stop();
            }

            currentTool.Process();
        }
    }
}