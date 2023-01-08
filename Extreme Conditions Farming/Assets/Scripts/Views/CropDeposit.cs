using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace ECF.Views
{
    public abstract class CropDeposit : MonoBehaviour, IToolTarget
    {
        public float ToolHeight => height;
        public Vector3 Position => dropPoint.position;
        
        [SerializeField] private Transform dropPoint;
        [SerializeField] private AudioClip[] depositSound;
        [SerializeField] private float height = 2;
        [SerializeField] private ParticleSystem depositEffect;
        [SerializeField] private GameObject hoverIndicator;

        private AudioSource source;

        protected virtual void Start()
        {
            source = GetComponent<AudioSource>();
            hoverIndicator.SetActive(false);
        }

        public void OnHoverBegan(Tool tool)
        {
            hoverIndicator.SetActive(true);
        }

        public void OnHoverEnded()
        {
            hoverIndicator.SetActive(false);
        }

        protected abstract bool TryDeposit(CropView cropView, out int result);
        
        public IToolUseResult UseTool(Tool tool)
        {
            if (tool is Hand hand)
            {
                if (hand.PickedUpResult == null)
                {
                    return null;
                }

                if (hand.PickedUpResult is CropView crop)
                {
                    if (TryDeposit(crop, out var amount))
                    {
                        hand.Release();
                        depositEffect.Emit(Mathf.Min(100, amount));
                        source.PlayOneShot(depositSound.Random());
                    }
                }
            }

            return null;
        }

    }
}