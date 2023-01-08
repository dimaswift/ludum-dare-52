using ECF.Behaviours;
using UnityEditor;
using UnityEngine;

namespace ECF.Views
{
    public class Hand : Tool
    { 
        [SerializeField] private Transform pickUpPoint;
        
        private static readonly int Holding = Animator.StringToHash("Holding");
        private IToolUseResult pickedUpResult;
        public IToolUseResult PickedUpResult => pickedUpResult; 
        
        public override void Process()
        {
            base.Process();
            if (pickedUpResult != null)
            {
                pickedUpResult.transform.SetParent(pickUpPoint);
                pickedUpResult.transform.SetPositionAndRotation(pickUpPoint.position, pickUpPoint.rotation);
            }
        }

        public void Release()
        {
            if (pickedUpResult == null)
            {
                return;
            }
            Destroy(pickedUpResult.transform.gameObject);
            pickedUpResult = null;
            Stop();
        }

        public override void Stop()
        {
            if (pickedUpResult != null)
            {
                return;
            }
            base.Stop();
        }

        public override bool CanActivate(IToolTarget target)
        {
            if (target is CropSlot slot)
            {
                if (pickedUpResult != null)
                {
                    return slot.Crop == null;
                }

                return slot.Crop != null;
            }

            if (target is Shop)
            {
                return pickedUpResult is CropView;
            }

            if (target is SeedConverter)
            {
                if (pickedUpResult is CropView crop)
                {
                    if (crop.Crop.Phase.CanConvertToSeeds())
                    {
                        return true;
                    }
                }
            
            }

            if (target is FamilyHome)
            {
                if (pickedUpResult is CropView crop)
                {
                    if (crop.Crop.Phase.CanEat())
                    {
                        return true;
                    }
                }

            }
            
            return false;
        }

        public override void Activate()
        {
            if (pickedUpResult == null)
            {
                base.Activate();
                return;
            }
            if (CanActivate(CurrentTarget))
            {
                Animator.SetBool(Holding, false);
                Animator.SetBool(Active, true);
            }
        }

        protected override void OnToolUsed(IToolUseResult result)
        {
            pickedUpResult = result;
            Animator.SetBool(Holding, true);
        }
    }
}