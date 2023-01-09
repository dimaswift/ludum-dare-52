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
        public int MaxWaterLevel => simulation.Config.MaxWaterLevel;
        
        public GardenBed Data => data;
        public ObservableValue<CropPhase> Phase { get; } = new(0);
        public ObservableValue<BedStatus> Status { get; } = new(0);
        public ObservableValue<int> ShapeLevel { get; } = new (0);
        public ObservableValue<int> WaterLevel { get; } = new(0);
        public ObservableValue<int> GrowthProgress { get; } = new(0);
        public ObservableValue<float> CurrentPhaseProgressNormalized { get; } = new(0);
        private CropTemplate template;
        private int nextPhaseProgress;
        private readonly ISimulation simulation;
        private readonly GardenBed data;

        private int waterDepletionCounter;
        
        public GardenBedBehaviour(ISimulation simulation, GardenBed data)
        {
            this.simulation = simulation;
            this.data = data;
            Status.Value = data.Status;
            ShapeLevel.Value = data.ShapeLevel;
            WaterLevel.Value = data.WaterLevel;
            GrowthProgress.Changed += p =>
            {
                CurrentPhaseProgressNormalized.Value = Mathf.Clamp01((float)p / nextPhaseProgress);
            };
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
                data.Crop.GrowProgress = CurrentPhaseProgressNormalized.Value;
                data.Crop.Phase = Phase.Value;
            }
        }

        public void OnInit(int time)
        {
            
        }

        public void OnDispose()
        {
            
        }
        
        private int CalculateWaterDepletionSpeed()
        {
            if (WaterLevel.Value == 0)
            {
                return 0;
            }

            if (WaterLevel.Value >= 20)
            {
                return 30;
            }

            if (WaterLevel.Value >= 10)
            {
                return 15;
            }
            
            return 3;
        }

        private int CalculateWaterConsumptionSpeed()
        {
            if (WaterLevel.Value > 20)
            {
                return 3;
            }

            if (WaterLevel.Value > 10)
            {
                return 2;
            }

            return 1;
        }

        
        public void OnTick(int time, int delta)
        {
            var depletionSpeed = CalculateWaterDepletionSpeed();
            
            waterDepletionCounter += delta * depletionSpeed;

            if (waterDepletionCounter > simulation.Config.GardenBedWaterCapacity)
            {
                waterDepletionCounter = 0;
                WaterLevel.Value = Math.Max(0, WaterLevel.Value - 1);
            }
            
            if (data == null || data.Crop == null || Status.Value != BedStatus.Planted)
            {
                return;
            }

            if (Phase.Value == CropPhase.Rotten)
            {
                return;
            }
            
            GrowthProgress.Value += CalculateWaterConsumptionSpeed() * delta;
            
            while (GrowthProgress.Value >= nextPhaseProgress)
            {
                GrowthProgress.Value -= nextPhaseProgress;
               
                if (Phase.Value == CropPhase.Rotten)
                {
                    nextPhaseProgress = template.PhaseStats.Durations[Phase.Value];
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

        public void PlaceCrop(Crop crop)
        {
            data.Crop = crop;
            template = simulation.CropTemplateFactory.Get(crop.Id);
            Status.Value = BedStatus.Planted;
            nextPhaseProgress = template.PhaseStats.Durations[data.Crop.Phase];
            Phase.Value = data.Crop.Phase;
            GrowthProgress.Value = (int) (crop.GrowProgress * nextPhaseProgress);
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
            crop = new Crop()
            {
                Id = template.Id,
                Attributes = data.Crop.Attributes,
                Genetics = data.Crop.Genetics,
                Phase = Phase.Value,
                GrowProgress = CurrentPhaseProgressNormalized.Value
            };
            data.Crop.Phase = Phase.Value;
            data.Crop = null;
            Status.Value = BedStatus.Empty;
            Phase.Value = CropPhase.Seed;
            ShapeLevel.Value = Math.Max(0, ShapeLevel.Value - 1);
            return true;
        }
    }
}