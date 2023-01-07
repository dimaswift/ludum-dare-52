using System.Collections.Generic;

namespace ECF.Domain
{
    public class CropTemplate
    {
        public string Name { get; set; }
        public int SellPrice { get; set; }
        public PhaseStats PhaseStats { get; set; }
    }

    public class PhaseStats
    {
        public Dictionary<CropPhase, int> Durations { get; set; }
        public Dictionary<CropPhase, int> WaterConsumption { get; set; }
        public Dictionary<CropPhase, int> SellPrices { get; set; }
        public Dictionary<CropPhase, int> SeedConversionRate { get; set; }
    }
    

    public enum CropPhase
    {
        Seed = 0,
        Sprout = 1,
        Bud = 2,
        Flower = 3,
        Green = 4,
        Unripe = 5,
        Ripe = 6,
        Overripe = 7,
        Rotten = 8
    }
}
