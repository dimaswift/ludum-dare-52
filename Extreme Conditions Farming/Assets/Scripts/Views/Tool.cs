using System;
using ECF.Behaviours;
using ECF.Domain;
using UnityEngine;

namespace ECF.Views
{
    public class Tool : MonoBehaviour
    {
        public IToolTarget CurrentTarget => currentTarget;

        [SerializeField] private ParticleSystem effect;

        public ToolType type;
        public Sprite icon;
        public AudioClip[] actionSounds;
        public string displayName;

        private Animator animator;
        private AudioSource audioSource;
        private static readonly int Active = Animator.StringToHash("Active");

        private IToolTarget currentTarget;
        private readonly RaycastHit[] raycastBuffer = new RaycastHit[10];

        private float noTargetTime;

        private Camera cam;

        public virtual void Init(ISimulation simulation)
        {
            
        }

        private void Awake()
        {
            cam = Camera.main;
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }

        public virtual void Activate()
        {
            if (!CanActivate())
            {
                return;
            }
            if (currentTarget == null || (!currentTarget.CanUseTool(this))) return;
            animator.SetBool(Active, true);
        }

        public virtual void Stop()
        {
            RemoveTarget();
            animator.SetBool(Active, false);
        }

        private void FixedUpdate()
        {
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            FindTarget(ray);
        }

        public void SetEffectMaterial(Material material)
        {
            effect.GetComponent<Renderer>().sharedMaterial = material;
        }

        public void RemoveTarget()
        {
            if (currentTarget != null)
            {
                currentTarget.OnHoverEnded();
                currentTarget = null;
            }
        }

        protected virtual bool CanActivate() => true;

        protected IToolTarget GetRaycastTarget(Ray ray)
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
                return target;
            }

            return null;
        }
        
        private void FindTarget(Ray ray)
        {
            var target = GetRaycastTarget(ray);
            if (target == null)
            {
                noTargetTime += Time.deltaTime;
                if (noTargetTime > 0.1f)
                {
                    Stop();
                }
                return;
            }

            if (target.CanUseTool(this))
            {
                currentTarget = target;
                if (Input.GetMouseButton(0))
                {
                    Activate();
                }
            }
        }
        
        public virtual void Process()
        {
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            var target = GetRaycastTarget(ray);
            if (target == null)
            {
                return;
            }

            if (target == currentTarget)
            {
                if (!currentTarget.CanUseTool(this))
                {
                    Stop();
                    return;
                }
            }
            
            if (target.CanUseTool(this))
            {
                currentTarget = target;
                currentTarget.OnHoverBegan(this);
            }
        }
        
        public void Perform()
        {
            if (currentTarget == null)
            {
                return;
            }

            currentTarget.UseTool(this);
            effect.Play(false);
            audioSource.PlayOneShot(actionSounds.Random());
        }
    }
}
