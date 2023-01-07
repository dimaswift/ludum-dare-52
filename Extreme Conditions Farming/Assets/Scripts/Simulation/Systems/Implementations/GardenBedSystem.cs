using System.Collections.Generic;
using ECF.Domain;
using ECF.Simulation.Behaviours;

namespace ECF.Simulation.Systems
{
    public class GardenBedSystem : BaseSystem, IGardenBedSystem
    {
        private readonly ISimulation simulation;
        private readonly ICropStorage cropStorage;

        private readonly HashSet<GardenBedBehaviour> beds = new();

        public class SaveData
        {
            public List<GardenBed> Beds { get; set; }
        }
        
        public GardenBedSystem(ISimulation simulation)
        {
            this.simulation = simulation;
            cropStorage = simulation.GetSystem<ICropStorage>();
            Load();
        }

        private void Load()
        {
            var data = simulation.Storage.Load(() => new SaveData()
            {
                Beds = new List<GardenBed>()
            });
            foreach (GardenBed bed in data.Beds)
            {
                AddBed(bed, out _);
            }
        }

        public bool AddBed(GardenBed data, out GardenBedBehaviour gardenBedBehaviour)
        {
            gardenBedBehaviour = new GardenBedBehaviour(simulation, data);
            beds.Add(gardenBedBehaviour);
            simulation.Add(gardenBedBehaviour);
            return true;
        }
        

        public override void SaveState()
        {
            base.SaveState();
            var data = new SaveData()
            {
                Beds = new List<GardenBed>()
            };
            foreach (GardenBedBehaviour bed in beds)
            {
                data.Beds.Add(bed.GetData());
            }
            simulation.Storage.Save(data);
        }

        public bool Harvest(GardenBedBehaviour gardenBed, out string error)
        {
            error = null;
            
            if (!beds.Contains(gardenBed))
            {
                error = "Bed not found";
                return false;
            }

            if (!gardenBed.TryHarvest(out var crop))
            {
                error = "Cannot harvest yet";
                return false;
            }
            
            beds.Remove(gardenBed);
            simulation.Remove(gardenBed);
            cropStorage.Add(crop);
            
            return true;
        }

        public IEnumerable<GardenBedBehaviour> GetBeds()
        {
            return beds;
        }
    }
}