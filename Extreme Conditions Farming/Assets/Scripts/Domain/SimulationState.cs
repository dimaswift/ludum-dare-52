namespace ECF.Domain
{
    public class SimulationState
    {
        public int Time { get; set; }
        public int RandomSeed { get; set; }
        public GardenBedSystemData GardenBeds { get; set; }
        public CropStorageData CropStorage { get; set; }
        public InventorySystemData Inventory { get; set; }
    }
}