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
        private readonly Dictionary<int, Crop> crops = new();
        private readonly ISimulation simulation;
        public event Action<Crop> OnCropAdded;

        public CropStorage(ISimulation simulation)
        {
            this.simulation = simulation;

            data = this.simulation.State.CropStorage;
            
            foreach (var crop in data.Crops)
            {
                while (crops.ContainsKey(crop.SlotNumber))
                {
                    crop.SlotNumber++;
                }
                crops.Add(crop.SlotNumber, crop);
            }
        }

        public override void SaveState()
        {
            base.SaveState();
            data.Crops.Clear();
            data.Crops.AddRange(crops.Values);
        }

        public bool HasRoom()
        {
            return crops.Count < Capacity.Value;
        }

        public Crop GetCropWithSlotNumber(int number)
        {
            return crops.TryGetValue(number, out var crop) ? crop : null;
        }

        public bool Sell(Crop crop, out int revenue)
        {
            revenue = 0;
            var template = simulation.CropTemplateFactory.Get(crop.Id);
            revenue = template.PhaseStats.SellPrices[crop.Phase];
            simulation.Inventory.Add(InventoryItems.Coins, revenue);
            return true;
        }

        public bool ConvertToSeeds(Crop crop, out int seeds)
        {
            seeds = 0;
            if (!crop.Phase.CanConvertToSeeds())
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
            var template = simulation.CropTemplateFactory.Get(crop.Id);
            calories = template.PhaseStats.NutritionRate[crop.Phase];
            var family = simulation.GetSystem<IFamilySystem>();
            family.Feed(calories);
            return true;
        }

        public IEnumerable<Crop> GetCrops()
        {
            return crops.Values;
        }
        
        public void Add(Crop crop, int slot)
        {
            var existingCrop = GetCropWithSlotNumber(slot);
            if (existingCrop != null)
            {
                Remove(slot, out _);
            }
            crop.SlotNumber = slot;
            crops.Add(slot, crop);
            OnCropAdded?.Invoke(crop);
        }
        
        public bool Remove(int slot, out Crop crop)
        {
            return crops.Remove(slot, out crop);
        }
    }
}