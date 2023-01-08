using System.Collections.Generic;

namespace ECF.Domain
{
    public class Crop
    {
        public string Id { get; set; }
        public CropPhase Phase { get; set; }
        public Genetics Genetics { get; set; }
        public List<CropAttribute> Attributes { get; set; }
    }
}