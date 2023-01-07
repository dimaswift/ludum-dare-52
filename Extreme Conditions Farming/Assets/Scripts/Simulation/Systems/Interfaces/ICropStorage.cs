using System.Collections.Generic;
using ECF.Domain;

namespace ECF.Simulation.Systems
{
    public interface ICropStorage : ISystem
    {
        void Add(Crop crop);
        bool Remove(Crop crop);

        bool Sell(Crop crop, out string error);

        bool ConvertToSeeds(Crop crop, out string error);
        IEnumerable<Crop> GetCrops();
    }
}