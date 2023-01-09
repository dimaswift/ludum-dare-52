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
        void Add(Crop crop, int slot);
        bool Remove(int slot, out Crop crop);
        Crop GetCropWithSlotNumber(int number);
        bool Sell(Crop crop, out int revenue);

        bool ConvertToSeeds(Crop crop, out int seedsAdded);
        bool FeedFamily(Crop crop, out int calories);
        IEnumerable<Crop> GetCrops();
        event Action<Crop> OnCropAdded;
    }
}