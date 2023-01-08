using System;
using System.Collections.Generic;
using ECF.Domain;
using ECF.Domain.Common;
using ECF.Behaviours.Systems;
using UnityEngine;

namespace ECF.Behaviours.Behaviours
{
    public class GardenBedBehaviour : ISimulated, IGardenBedBehaviour
    {
        public int MaxWaterLevel => 9;
        
        public GardenBed Data => data;
        public ObservableValue<CropPhase> Phase { get; } = new(0);
        public ObservableValue<BedStatus> Status { get; } = new(0);
        public ObservableValue<int> ShapeLevel { get; } = new (0);
        public ObservableValue<int> WaterLevel { get; } = new(0);
        public ObservableValue<int> GrowthProgress { get; } = new(0);
        private CropTemplate template;
        private int nextPhaseProgress;
        private readonly ISimulation simulation;
        private readonly ICropStorage cropStorage;
        private readonly GardenBed data;

        private int waterDepletionCounter;
        

        public GardenBedBehaviour(ISimulation simulation, GardenBed data)
        {
            this.simulation = simulation;
            this.data = data;
            Status.Value = data.Status;
            ShapeLevel.Value = data.ShapeLevel;
            WaterLevel.Value = data.WaterLevel;
            cropStorage = this.simulation.GetSystem<ICropStorage>();
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
            data.Status = Status.Value;
            if (data.Crop != null)
            {
                data.Crop.Phase = Phase.Value;
            }
        }

        public void OnInit(int time)
        {
            
        }

        public void OnDispose()
        {
            
        }

        private int CalculateGrowRate(int delta)
        {
            if (WaterLevel.Value == 0)
            {
                return 0;
            }
            
            return (int) Math.Ceiling(delta * ((float)WaterLevel.Value / MaxWaterLevel));
        }
        
        public void OnTick(int time, int delta)
        {
            waterDepletionCounter += delta;

            if (waterDepletionCounter > 30)
            {
                waterDepletionCounter = 0;
                WaterLevel.Value = Math.Max(0, WaterLevel.Value - 1);
            }
            
            if (data == null || data.Crop == null || Status.Value != BedStatus.Planted)
            {
                return;
            }

            GrowthProgress.Value += CalculateGrowRate(delta);
            
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
            
            if (!simulation.Inventory.Use(InventoryItems.Coins, data.UnlockPrice))
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
            Status.Value = BedStatus.Planted;
            nextPhaseProgress = template.PhaseStats.Durations[data.Crop.Phase];
            Phase.Value = data.Crop.Phase;
            ShapeLevel.Value--;
        }

        public void ImproveShape()
        {
            ShapeLevel.Value++;
        }

        public void Water(int amount)
        {
            WaterLevel.Value += amount;
            waterDepletionCounter = 0;
        }
        
        public bool Plant(CropTemplate template, out Crop crop, out string error)
        {
            error = null;
            crop = null;
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
            
            crop = new Crop()
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

            if (!Phase.Value.IsHarvestable())
            {
                return false;
            }

            if (!cropStorage.HasRoom())
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
            data.Crop.Phase = Phase.Value;
            cropStorage.Add(data.Crop);
            data.Crop = null;
            Status.Value = BedStatus.Empty;
            Phase.Value = CropPhase.Seed;
            nextPhaseProgress = 0;
            ShapeLevel.Value = 0;
            return true;
        }
    }
}