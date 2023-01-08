namespace ECF.Domain
{
    public class CropTemplate
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string SeedId { get; set; }
        public string HarvestId { get; set; }
        public PhaseStats PhaseStats { get; set; }
    }
}
