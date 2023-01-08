using ECF.Domain;

namespace ECF.Views
{
    public class Hoe : Tool
    {
        public override bool CanActivate(IToolTarget target)
        {
            if (target is GardenBedView bed)
            {
                if (bed.Behaviour.Status.Value == BedStatus.Locked)
                {
                    return false;
                }

                return bed.Behaviour.Status.Value == BedStatus.Empty &&
                       bed.Behaviour.ShapeLevel.Value <  bed.ShapesCount - 1;
            }

            return false;

        }
    }
}