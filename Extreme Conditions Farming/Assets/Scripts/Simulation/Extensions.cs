using ECF.Domain;

namespace ECF.Simulation
{
    public static class Extensions
    {
        public static bool IsHarvestable(this CropPhase phase)
        {
            switch (phase)
            {
                case CropPhase.Ripe:
                case CropPhase.Overripe:
                case CropPhase.Unripe:
                case CropPhase.Green:
                case CropPhase.Rotten:
                    return true;
                default:
                    return false;
            }
        }
    }
}