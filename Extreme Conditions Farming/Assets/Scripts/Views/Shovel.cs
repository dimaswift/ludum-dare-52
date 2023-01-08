using ECF.Behaviours;
using ECF.Behaviours.Systems;
using ECF.Domain;

namespace ECF.Views
{
    public class Shovel : Tool
    {
        public override bool CanActivate(IToolTarget target)
        {
            if (target is GardenBedView bed)
            {
                var storage = Game.Instance.Simulation.GetSystem<ICropStorage>();
                if (!storage.HasRoom())
                {
                    return false;
                }
                return bed.Behaviour.Status.Value == BedStatus.Planted && bed.Behaviour.Phase.Value.IsHarvestable();
            }

            return false;
        }
    }
}