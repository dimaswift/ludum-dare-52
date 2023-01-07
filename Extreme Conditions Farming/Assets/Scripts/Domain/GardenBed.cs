namespace ECF.Domain
{
    public class GardenBed
    {
        public int Number { get; set; }
        public BedStatus Status{ get; set; }
        public Crop Crop { get; set; }
        public int GrowthProgress { get; set; }
        public int WaterLevel { get; set; }
        public int AcidLevel { get; set; }
        public int RadiationLevel { get; set; }
        public int ShapeLevel { get; set; }
        public int UnlockPrice { get; set; }
    }
}