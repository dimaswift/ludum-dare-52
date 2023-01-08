using System;
using UnityEngine;

namespace ECF.Views
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 10;
        [SerializeField] private float drag = 10;
        private Transform camTransform;

        private bool isDragging;
        private Camera cam;
        private Vector3 clickMousePos;
        private Vector3 clickCameraPos;
        private Plane plane = new Plane(Vector3.up, Vector3.zero);

        private Vector3 velocity;
        private float ratio;
        
        private void Awake()
        {
            cam = Camera.main;
            camTransform = cam.transform;
            ratio = (GetMouseWorldPosition(Vector2.zero).x - GetMouseWorldPosition(new Vector2(
                0, Screen.height)).x) / Screen.height;
        }

        private Vector3 GetMouseWorldPosition(Vector2 screenPos)
        {
            Ray ray = cam.ScreenPointToRay(screenPos);
            plane.Raycast(ray, out var distance);
            return ray.GetPoint(distance);
            
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                isDragging = true;
                clickCameraPos = camTransform.position;
                clickMousePos = Input.mousePosition;
            }
            
            if (Input.GetMouseButtonUp(1))
            {
                isDragging = false;
            }

            if (isDragging)
            {
                var delta = Input.mousePosition - clickMousePos;
                var oldPos = camTransform.position;
                var newPos = clickCameraPos - transform.TransformDirection(new Vector3(delta.x, delta.y, 0)) * ratio;
                velocity = (newPos - oldPos) * moveSpeed;
            }
            else
            {
                velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * drag);
            }

            camTransform.position += velocity * Time.deltaTime;
        }
    }
}