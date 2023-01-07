using System.Collections.Generic;
using ECF.Domain;
using ECF.Simulation.Behaviours;

namespace ECF.Simulation.Systems
{
    public interface IGardenBedSystem : ISystem
    {
        bool AddBed(GardenBed data, out GardenBedBehaviour gardenBed);
        bool Harvest(GardenBedBehaviour gardenBed, out string error);
        IEnumerable<GardenBedBehaviour> GetBeds();
    }
}