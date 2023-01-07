using System;
using System.Collections.Generic;
using ECF.Domain;
using UnityEngine;
using UnityEngine.Serialization;

namespace ECF.Views
{
    public interface IToolTarget
    {
        bool CanUseTool(Tool tool);
        void OnHoverBegan(Tool tool);
        void OnHoverEnded();
        void UseTool(Tool tool);
    }
    
    public class Tool : MonoBehaviour
    {
        [SerializeField] private ParticleSystem effect;

        public ToolType type;
        public Sprite icon;
        public AudioClip[] actionSounds;
        public string displayName;

        private Animator animator;
        private AudioSource audioSource;
        private static readonly int Active = Animator.StringToHash("Active");

        private IToolTarget currentTarget;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }

        public void Activate()
        {
            if (currentTarget == null || !currentTarget.CanUseTool(this)) return;
            
            animator.SetBool(Active, true);
        }

        public void Stop()
        {
            RemoveTarget();
            animator.SetBool(Active, false);
        }

        public void RemoveTarget()
        {
            if (currentTarget != null)
            {
                currentTarget.OnHoverEnded();
                currentTarget = null;
            }
        }

        public bool TrySetTarget(IToolTarget target)
        {
            if (currentTarget == target)
            {
                return target.CanUseTool(this);
            }

            if (target.CanUseTool(this))
            {
                target.OnHoverBegan(this);
                if (currentTarget != null)
                {
                    currentTarget.OnHoverEnded();
                }
                currentTarget = target;
                return true;
            }

            return false;
        }
        
        public void Perform()
        {
            if (currentTarget == null)
            {
                return;
            }

            if (!currentTarget.CanUseTool(this))
            {
                Stop();
                return;
            }
            currentTarget.UseTool(this);
            effect.Play(false);
            audioSource.PlayOneShot(actionSounds.Random());
        }
    }

}
