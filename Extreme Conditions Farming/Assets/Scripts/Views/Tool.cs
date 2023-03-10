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

        protected Animator Animator { get; private set; }
        private AudioSource audioSource;
        protected static readonly int Active = Animator.StringToHash("Active");

        private IToolTarget currentTarget;
        private readonly RaycastHit[] raycastBuffer = new RaycastHit[10];

        private float noTargetTime;
        private Camera cam;
   
        public virtual void Init(ISimulation simulation)
        {
            currentTarget = null;
            Stop();
        }
        
        private void Awake()
        {
            cam = Camera.main;
            Animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }
        
        private void OnEnable()
        {
            Move(cam.ScreenPointToRay(Input.mousePosition), null, false);
        }

        public virtual void Activate()
        {
            Animator.SetBool(Active, true);
        }

        public virtual void Stop()
        {
            RemoveTarget();
            if (Animator == null)
            {
                return;
            }
           
            Animator.SetBool(Active, false);
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

        public virtual bool CanActivate(IToolTarget target)
        {
            return true;
        }

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

            if (currentTarget != null)
            {
                var targetPos = currentTarget.Position;
                pos = new Vector3(targetPos.x, currentTarget.ToolHeight, targetPos.z);
                if (currentTarget is IToolUseResult toolUseResult)
                {
                    pos.y = toolUseResult.HoldPoint.position.y;
                }
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
                if (!CanActivate(target))
                {
                    Stop();
                }
                return;
            }
            
            RemoveTarget();
            
            if (CanActivate(target))
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
            
        }

        public void OnAnimationFinished()
        {
            
        }

        protected virtual void OnToolUsed(IToolUseResult result) {}
        
        public void Perform()
        {
            if (currentTarget == null)
            {
                return;
            }

            var result = currentTarget.UseTool(this);
            if (result != null)
            {
                OnToolUsed(result);
            }
            effect.Play(false);
            audioSource.PlayOneShot(actionSounds.Random());
        }
    }
}
