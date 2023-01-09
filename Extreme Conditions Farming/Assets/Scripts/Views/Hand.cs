using ECF.Behaviours;
using ECF.Domain;
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

        public override void Init(ISimulation simulation)
        {
            base.Init(simulation);
            
            if (Animator != null)
            {
                Animator.SetBool(Holding, false);
            }
          
            if (pickedUpResult != null)
            {
                Destroy(pickedUpResult.transform.gameObject);
                pickedUpResult = null;
            }
        }

        public override void Process()
        {
            base.Process();
            if (pickedUpResult != null)
            {
                pickedUpResult.transform.SetParent(pickUpPoint);
                pickedUpResult.transform.SetPositionAndRotation(pickUpPoint.position, pickUpPoint.rotation);
                pickedUpResult.transform.Translate(-pickedUpResult.HoldPoint.localPosition * pickedUpResult.HoldPoint.parent.localScale.x, Space.Self);
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

        private bool CanActivateGardenBed(GardenBedView bed)
        {
            if (bed.Behaviour.Status.Value == BedStatus.Locked)
            {
                if (PickedUpResult != null)
                {
                    return false;
                }

                var coins = Game.Instance.Simulation.Inventory.Get(InventoryItems.Coins).Value;
                return bed.Behaviour.Data.UnlockPrice <= coins;
            }

            if (PickedUpResult != null)
            {
                if (PickedUpResult is CropView crop)
                {
                    return bed.Behaviour.Status.Value == BedStatus.Empty && crop.Crop.Phase.CanPlant();
                }

                return false;
            }

            return bed.Behaviour.Status.Value == BedStatus.Planted;
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

            if (target is GardenBedView bed)
            {
                return CanActivateGardenBed(bed);
            }

            if (target is SeedShop)
            {
                return PickedUpResult == null;
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