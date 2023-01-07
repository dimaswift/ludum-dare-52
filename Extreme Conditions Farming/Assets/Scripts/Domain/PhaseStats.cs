using System.Collections.Generic;
using ECF.Domain.Common;

namespace ECF.Domain
{
    public class PhaseStats
    {
        public Dictionary<CropPhase, int> Durations { get; set; }
        public Dictionary<CropPhase, int> WaterConsumption { get; set; }
        public Dictionary<CropPhase, int> SellPrices { get; set; }
        public Dictionary<CropPhase, int> SeedConversionRate { get; set; }
    }
}