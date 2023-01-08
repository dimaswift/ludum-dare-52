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
        private bool isAnimating;

        public virtual void Init(ISimulation simulation)
        {
            
        }

        private void Awake()
        {
            cam = Camera.main;
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            Move(cam.ScreenPointToRay(Input.mousePosition), null, false);
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
        

        private void Move(Ray ray, IToolTarget hoveringOverTarget, bool lerp)
        {
            float y = 0;

            if (hoveringOverTarget != null)
            {
                y = hoveringOverTarget.ToolHeight;
            }

            var plane = new Plane(Vector3.up, new Vector3(0, y, 0));
            
            plane.Raycast(ray, out var distance);

            var pos = ray.GetPoint(distance);

            if (isAnimating && currentTarget != null)
            {
                var targetPos = currentTarget.Position;
                pos = new Vector3(targetPos.x, 0, targetPos.z);
            }
            
            transform.position = lerp ? Vector3.Lerp(transform.position, pos, Time.deltaTime * 25) : pos;
        }
        
        public virtual void Process()
        {
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            
            var target = GetRaycastTarget(ray);

            Move(ray, target, true);
            
            if (target == null)
            {
                RemoveTarget();
                return;
            }

            if (target == currentTarget)
            {
                if (!currentTarget.CanUseTool(this))
                {
                    Stop();
                }
                return;
            }
            
            RemoveTarget();
            
            if (target.CanUseTool(this))
            {
                currentTarget = target;
                currentTarget.OnHoverBegan(this);
                if (Input.GetMouseButton(0))
                {
                    Activate();
                }
            }
            else
            {
                Stop();
            }
        }

        public void OnAnimationStarted()
        {
            isAnimating = true;
        }

        public void OnAnimationFinished()
        {
            isAnimating = false;
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
