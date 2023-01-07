using System.Collections.Generic;
using ECF.Domain;
using ECF.Domain.Common;
using ECF.Behaviours.Systems;

namespace ECF.Behaviours.Behaviours
{
    public class GardenBedBehaviour : ISimulated, IGardenBedBehaviour
    {
        public GardenBed Data => data;
        public ObservableValue<CropPhase> Phase { get; } = new(0);
        public ObservableValue<BedStatus> Status { get; } = new(0);
        public ObservableValue<int> ShapeLevel { get; } = new (0);
        public ObservableValue<int> WaterLevel { get; } = new(0);
        public ObservableValue<int> GrowthProgress { get; } = new(0);
        private CropTemplate template;
        private int nextPhaseProgress;
        private readonly ISimulation simulation;
        private readonly GardenBed data;

        public GardenBedBehaviour(ISimulation simulation, GardenBed data)
        {
            this.simulation = simulation;
            this.data = data;
            Status.Value = data.Status;
            ShapeLevel.Value = data.ShapeLevel;
            WaterLevel.Value = data.WaterLevel;
            if (data.Crop != null)
            {
                PlaceCrop(data.Crop);
            }
        }

        public void Save()
        {
            data.GrowthProgress = GrowthProgress.Value;
            data.ShapeLevel = ShapeLevel.Value;
            data.WaterLevel = WaterLevel.Value;
        }

        public void OnInit(int time)
        {
            
        }

        public void OnDispose()
        {
            
        }

        public void OnTick(int time, int delta)
        {
            if (data == null || data.Crop == null || Status.Value != BedStatus.Planted)
            {
                return;
            }

            GrowthProgress.Value += delta;
            
            while (GrowthProgress.Value >= nextPhaseProgress)
            {
                GrowthProgress.Value -= nextPhaseProgress;
               
                if (Phase.Value == CropPhase.Rotten)
                {
                    break;
                }

                Phase.Value = (CropPhase)((int)Phase.Value + 1);
                nextPhaseProgress = template.PhaseStats.Durations[Phase.Value];
            }
        }
        
        public void AddAttribute(CropAttribute attribute)
        {
            if (data.Crop == null)
            {
                return;
            }
            if (!data.Crop.Attributes.Contains(attribute))
            {
                data.Crop.Attributes.Add(attribute);
            }
        }

        public void RemoveAttribute(CropAttribute attribute)
        {
            data.Crop.Attributes.Remove(attribute);
        }

        public bool HasAttribute(CropAttribute attribute)
        {
            return data.Crop.Attributes.Contains(attribute);
        }

        
        public bool Unlock()
        {
            if (data.Status != BedStatus.Locked)
            {
                return false;
            }
            
            if (!simulation.Inventory.Use(Resources.Coins, data.UnlockPrice))
            {
                return false;
            }

            Status.Value = BedStatus.Empty;
            return true;
        }

        private Genetics CalculateGenetics()
        {
            return Genetics.Average;
        }

        private void PlaceCrop(Crop crop)
        {
            data.Crop = crop;
            template = simulation.CropTemplateFactory.Get(crop.Id);
            Status.Value = crop.Phase.IsHarvestable() ? BedStatus.Harvestable : BedStatus.Planted;
            nextPhaseProgress = template.PhaseStats.Durations[data.Crop.Phase];
            Phase.Value = data.Crop.Phase;
        }

        public void ImproveShape()
        {
            ShapeLevel.Value++;
        }
        
        public bool Plant(CropTemplate template, out string error)
        {
            error = null;
            
            if (Status.Value != BedStatus.Empty)
            {
                error = $"Garden bed #{data.Number} is not ready to plant";
                return false;
            }
            
            if (data.Crop != null)
            {
                error = $"Garden bed #{data.Number} already has a crop";
                return false;
            }

            if (!simulation.Inventory.Use(template.SeedId, 1))
            {
                error = $"You don't have any {template.SeedId} in your inventory";
                return false;
            }
            
            var crop = new Crop()
            {
                Id = template.Id,
                Attributes = new List<CropAttribute>(),
                Genetics = CalculateGenetics(),
                Phase = CropPhase.Seed
            };
            PlaceCrop(crop);
            return true;
        }
        
        public bool TryHarvest(out Crop crop)
        {
            crop = null;
            if (data.Crop == null)
            {
                return false;
            }

            if (!data.Crop.Phase.IsHarvestable())
            {
                return false;
            }

            crop = new Crop()
            {
                Id = template.HarvestId,
                Attributes = data.Crop.Attributes,
                Genetics = data.Crop.Genetics,
                Phase = Phase.Value
            };
            data.Crop = null;
            Status.Value = BedStatus.Harvested;
            nextPhaseProgress = 0;
            return true;
        }
    }
}