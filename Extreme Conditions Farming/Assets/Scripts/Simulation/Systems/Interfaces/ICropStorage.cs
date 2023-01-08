using System;
using System.Collections.Generic;
using ECF.Domain;
using ECF.Domain.Common;

namespace ECF.Behaviours.Systems
{
    public interface ICropStorage : ISystem
    {
        bool HasRoom();
        ObservableValue<int> Capacity { get; }
        void Add(Crop crop);
        bool Remove(Crop crop);

        bool Sell(Crop crop, out string error);

        bool ConvertToSeeds(Crop crop, out string error);
        IEnumerable<Crop> GetCrops();
        event Action<Crop> OnCropAdded;
    }
}