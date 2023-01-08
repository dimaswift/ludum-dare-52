using System.Collections.Generic;
using ECF.Behaviours.Behaviours;
using ECF.Domain;
using UnityEngine;

namespace ECF.Behaviours.Systems
{
    public class GardenBedSystem : BaseSystem, IGardenBedSystem
    {
        private readonly ISimulation simulation;
        private readonly ICropStorage cropStorage;
        
        private readonly HashSet<GardenBedBehaviour> beds = new();
        public GardenBedSystem(ISimulation simulation)
        {
            this.simulation = simulation;
            cropStorage = simulation.GetSystem<ICropStorage>();
            Load(simulation.State.GardenBeds);
        }

        private void Load(GardenBedSystemData data)
        {
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
            foreach (GardenBedBehaviour bed in beds)
            {
                bed.Save();
            }
        }

        public IEnumerable<GardenBedBehaviour> GetBeds()
        {
            return beds;
        }
    }
}