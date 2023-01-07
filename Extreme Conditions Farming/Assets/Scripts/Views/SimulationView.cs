using ECF.Behaviours;
using ECF.Behaviours.Behaviours;
using ECF.Behaviours.Systems;
using UnityEngine;

namespace ECF.Views
{
    public class SimulationView : MonoBehaviour
    {
        [SerializeField] private GardenBedView[] gardenBeds;
        private ISimulation simulation;
        
        public void Dispose()
        {
            Destroy(gameObject);
        }

        public void Init(ISimulation simulation)
        {
            this.simulation = simulation;
            var bedSystem = simulation.GetSystem<IGardenBedSystem>();
            foreach (GardenBedBehaviour bedBehaviour in bedSystem.GetBeds())
            {
                var bedView = gardenBeds[bedBehaviour.Data.Number];
                bedView.Init(bedBehaviour);
            }
        }
    }
}