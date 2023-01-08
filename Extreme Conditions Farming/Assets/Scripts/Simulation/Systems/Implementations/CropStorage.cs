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
        
        public bool Sell(Crop crop, out int revenue)
        {
            revenue = 0;
            if (Remove(crop))
            {
                var template = simulation.CropTemplateFactory.Get(crop.Id);
                revenue = template.PhaseStats.SellPrices[crop.Phase];
                simulation.Inventory.Add(InventoryItems.Coins, revenue);
                return true;
            }
            return false;
        }

        public bool ConvertToSeeds(Crop crop, out int seeds)
        {
            seeds = 0;
            if (!crop.Phase.CanConvertToSeeds())
            {
                return false;
            }
            if (!Remove(crop))
            {
                return false;
            }
            var template = simulation.CropTemplateFactory.Get(crop.Id);
            seeds = template.PhaseStats.SeedConversionRate[crop.Phase];
            simulation.Inventory.Add(template.SeedId, seeds);
            return true;
        }

        public bool FeedFamily(Crop crop, out int calories)
        {
            calories = 0;
            if (!Remove(crop))
            {
                return false;
            }

            var template = simulation.CropTemplateFactory.Get(crop.Id);
            calories = template.PhaseStats.NutritionRate[crop.Phase];
            var family = simulation.GetSystem<IFamilySystem>();
            family.Feed(calories);
            return true;
        }

        public IEnumerable<Crop> GetCrops()
        {
            return crops;
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