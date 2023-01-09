using System.Collections.Generic;

namespace ECF.Domain
{
    public class SimulationConfig
    {
        public int GardenBedCount { get; set; } = 28;
        public float BedUnlockPriceMultiplier { get; set; } = 1.3f;
        public int BaseUnlockPrice { get; set; } = 10;
        public int UnlockedBedsAmount { get; set; } = 6;
        public int StartCoins { get; set; } = 100;
        public int GardenBedWaterCapacity { get; set; } = 100;
        public int MaxWaterLevel { get; set; } = 9;
        public int SeedPrice { get; set; } = 5;
        public int WateringCanWaterAmount { get; set; } = 5;
        public List<CropTemplate> Templates { get; set; } = new();
        public List<InventoryItemData> StartItems { get; set; } = new();

    }
}