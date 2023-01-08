using ECF.Behaviours;
using ECF.Domain;

namespace ECF.Views
{
    public class Shovel : Tool
    {
        public override bool CanActivate(IToolTarget target)
        {
            if (target is GardenBedView bed)
            {
                return bed.Behaviour.Status.Value == BedStatus.Planted && bed.Behaviour.Phase.Value.IsHarvestable();
            }

            return false;
        }
    }
}