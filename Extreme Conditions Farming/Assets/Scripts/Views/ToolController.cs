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

        private Camera cam;

        private Plane plane = new (Vector3.up, Vector3.zero);

        private float noTargetTime;
        
        private void Awake()
        {
            Game.Instance.OnNewSimulationCreated += OnNewSimulationCreated;
            Tools = GetComponentsInChildren<Tool>();
            cam = Camera.main;
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
                currentTool.Activate();
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                currentTool.Stop();
            }

            currentTool.Process();
            
            MoveTool(cam.ScreenPointToRay(Input.mousePosition));
        }

        
        void MoveTool(Ray ray)
        {
            plane.Raycast(ray, out var distance);

            var pos = ray.GetPoint(distance);

            currentTool.transform.position = pos;
        }
    }
}