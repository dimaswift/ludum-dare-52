using System;
using UnityEngine;

namespace ECF.Views
{
    public class CropPhaseView : MonoBehaviour
    {
        public Transform HoldPoint => holdPoint;
        
        private Transform holdPoint;
        [SerializeField] private float fromScale = 1;
        [SerializeField] private float toScale = 1;
        [SerializeField] private float baseScale = 1;
        private void Awake()
        {
            holdPoint = transform.Find("HoldPoint");
            transform.localScale = new Vector3(fromScale, fromScale, fromScale) * baseScale;
        }
        

        public void SetContinuousProgress(float progress)
        {
            var scale = Mathf.Lerp(fromScale, toScale, Mathf.Clamp01(progress));
            transform.localScale = new Vector3(scale, scale, scale) * baseScale;
        }
    }
}