using System;
using System.Collections.Generic;
using ECF.Domain;
using ECF.Domain.Common;

namespace ECF.Behaviours.Systems
{
    public class CropStorage : BaseSystem, ICropStorage
    {
        public ObservableValue<int> Capacity { get; } = new(4);

        private readonly CropStorageData data;
        private readonly HashSet<Crop> crops = new();
        private readonly ISimulation simulation;
        public event Action<Crop> OnCropAdded;

        public CropStorage(ISimulation simulation)
        {
            this.simulation = simulation;

            data = this.simulation.State.CropStorage;
            
            foreach (var crop in data.Crops)
            {
                crops.Add(crop);
            }
        }

        public override void SaveState()
        {
            base.SaveState();
            data.Crops.Clear();
            data.Crops.AddRange(crops);
        }

        public bool HasRoom()
        {
            return crops.Count < Capacity.Value;
        }
        
        public bool Sell(Crop crop, out string error)
        {
            error = null;
            if (Remove(crop))
            {
                var template = simulation.CropTemplateFactory.Get(crop.Id);
                simulation.Inventory.Add(Resources.Coins, template.PhaseStats.SellPrices[crop.Phase]);
                return true;
            }
            error = "Crop not found";
            return false;
        }

        public bool ConvertToSeeds(Crop crop, out string error)
        {
            error = null;
            if (!CanConvertToSeeds(crop.Phase))
            {
                error = "Crop is not mature";
                return false;
            }
            if (!Remove(crop))
            {
                error = "Crop not found";
                return false;
            }
            var template = simulation.CropTemplateFactory.Get(crop.Id);
            simulation.Inventory.Add(template.SeedId, template.PhaseStats.SeedConversionRate[crop.Phase]);
            return true;
        }

        public IEnumerable<Crop> GetCrops()
        {
            return crops;
        }


        private bool CanConvertToSeeds(CropPhase phase)
        {
            switch (phase)
            {
                case CropPhase.Ripe:
                case CropPhase.Overripe:
                case CropPhase.Rotten:
                    return true;
                default:
                    return false;
            }
        }

        public void Add(Crop crop)
        {
            crops.Add(crop);
            OnCropAdded?.Invoke(crop);
        }

        public bool Remove(Crop crop)
        {
            return crops.Remove(crop);
        }
    }
}