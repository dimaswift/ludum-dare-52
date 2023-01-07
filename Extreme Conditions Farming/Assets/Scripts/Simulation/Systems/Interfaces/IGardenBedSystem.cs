using System.Collections.Generic;
using ECF.Behaviours.Behaviours;
using ECF.Domain;

namespace ECF.Behaviours.Systems
{
    public interface IGardenBedSystem : ISystem
    {
        bool AddBed(GardenBed data, out GardenBedBehaviour gardenBed);
        bool Harvest(GardenBedBehaviour gardenBed, out string error);
        IEnumerable<GardenBedBehaviour> GetBeds();
    }
}