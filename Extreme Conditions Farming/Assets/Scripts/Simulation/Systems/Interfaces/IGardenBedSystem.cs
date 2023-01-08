using System.Collections.Generic;
using ECF.Behaviours.Behaviours;
using ECF.Domain;

namespace ECF.Behaviours.Systems
{
    public interface IGardenBedSystem : ISystem
    {
        IEnumerable<GardenBedBehaviour> GetBeds();
    }
}