using ECF.Domain;

namespace ECF.Views
{
    public class WateringCan : Tool
    {
        public override bool CanActivate(IToolTarget target)
        {
            if (target is GardenBedView bed)
            {
                return bed.Behaviour.Status.Value != BedStatus.Locked && bed.Behaviour.WaterLevel.Value < bed.Behaviour.MaxWaterLevel;
            }

            return false;
        }
    }
}