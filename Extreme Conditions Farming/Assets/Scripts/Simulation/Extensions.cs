using ECF.Domain;

namespace ECF.Behaviours
{
    public static class Extensions
    {
        public static bool CanConvertToSeeds(this CropPhase phase)
        {
            switch (phase)
            {
                case CropPhase.Ripe:
                case CropPhase.Overripe:
                case CropPhase.Rotten:
                    return true;
                default:
                    return false;
            }
        }

        public static bool CanEat(this CropPhase phase)
        {
            switch (phase)
            {
                case CropPhase.Unripe:
                case CropPhase.Ripe:
                case CropPhase.Overripe:
                    return true;
                default:
                    return false;
            }
        }

        public static bool CanPlant(this CropPhase phase)
        {
            switch (phase)
            {
                case CropPhase.Seed:
                case CropPhase.Bud:
                case CropPhase.Flower:
                case CropPhase.Sprout:
                    return true;
                default:
                    return false;
            }
        }
    }
}