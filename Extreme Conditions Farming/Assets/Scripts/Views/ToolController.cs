using System;
using UnityEngine;

namespace ECF.Views
{

    public class ToolController : MonoBehaviour
    {
        public event Action OnToolChanged; 
        public Tool Current => currentTool;
        public Tool[] Tools => tools;
        
        private Tool currentTool;
        private readonly RaycastHit[] raycastBuffer = new RaycastHit[10];

        [SerializeField] private Tool[] tools;

        private Camera cam;

        private Plane plane = new (Vector3.up, Vector3.zero);

        private void Awake()
        {
            cam = Camera.main;
            currentTool = tools[0];
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
        }

        private void FindTarget(Ray ray)
        {
            var hits = Physics.RaycastNonAlloc(ray, raycastBuffer, 100f);
            for (int i = 0; i < hits; i++)
            {
                var hit = raycastBuffer[i];
                var target = hit.collider.GetComponent<IToolTarget>();
                if (target == null)
                {
                    continue;
                }
                if (currentTool.TrySetTarget(target))
                {
                    if (Input.GetMouseButton(0))
                    {
                        currentTool.Activate();
                    }
                    return;
                }
            }

            currentTool.Stop();
        }

        void MoveTool(Ray ray)
        {
            plane.Raycast(ray, out var distance);

            var pos = ray.GetPoint(distance);

            currentTool.transform.position = pos;
        }
        
        void FixedUpdate()
        {
            if (currentTool == null)
            {
                return;
            }
            var ray = cam.ScreenPointToRay(Input.mousePosition);

            MoveTool(ray);

            FindTarget(ray);
        }
    }
}