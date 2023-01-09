namespace ECF.Domain
{
    public class CropTemplate
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string SeedId =>  $"{Id}_seed";
        public PhaseStats PhaseStats { get; set; }
    }
}
