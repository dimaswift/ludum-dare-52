using System.Collections.Generic;
using ECF.Domain;
using ECF.Domain.Common;
using ECF.Behaviours.Systems;

namespace ECF.Behaviours.Behaviours
{
    public class GardenBedBehaviour : ISimulated, IGardenBedBehaviour
    {
        public GardenBed GetData() => data;
        
        public IObservableValue<BedStatus> Status => status;
        public IObservableValue<CropPhase> Phase => phase;
        private CropTemplate template;
        private readonly ObservableValue<BedStatus> status;
        private readonly ObservableValue<CropPhase> phase;
        private int nextPhaseProgress;
        private readonly ISimulation simulation;
        private readonly GardenBed data;

        public GardenBedBehaviour(ISimulation simulation, GardenBed data)
        {
            this.simulation = simulation;
            status = new ObservableValue<BedStatus>(data.Status);
            phase = new ObservableValue<CropPhase>(CropPhase.Seed);
            this.simulation = simulation;
            this.data = data;
            if (data.Crop != null)
            {
                PlaceCrop(data.Crop);
            }
        }

        public void OnInit(int time)
        {
            
        }

        public void OnDispose()
        {
            
        }

        public void OnTick(int time, int delta)
        {
            if (data == null || data.Crop == null || status.Value != BedStatus.Planted)
            {
                return;
            }

            data.GrowthProgress += delta;
            
            while (data.GrowthProgress >= nextPhaseProgress)
            {
                data.GrowthProgress -= nextPhaseProgress;
               
                if (phase.Value == CropPhase.Rotten)
                {
                    break;
                }

                phase.Value = (CropPhase)((int)phase.Value + 1);
                nextPhaseProgress = template.PhaseStats.Durations[phase.Value];
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

            status.Value = BedStatus.Empty;
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
            status.Value = crop.Phase.IsHarvestable() ? BedStatus.Harvestable : BedStatus.Planted;
            nextPhaseProgress = template.PhaseStats.Durations[data.Crop.Phase];
            phase.Value = data.Crop.Phase;
        }
        
        public bool Plant(CropTemplate template, out string error)
        {
            error = null;
            
            if (status.Value != BedStatus.Empty)
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
                Phase = phase.Value
            };
            data.Crop = null;
            status.Value = BedStatus.Harvested;
            nextPhaseProgress = 0;
            return true;
        }
    }
}