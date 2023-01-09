using System.Collections.Generic;
using ECF.Behaviours;
using ECF.Behaviours.Behaviours;
using ECF.Behaviours.Systems;
using UnityEngine;

namespace ECF.Views
{
    public class SimulationView : MonoBehaviour
    {
        [SerializeField] private GardenBedView[] gardenBeds;
   
        public void Dispose()
        {
            Destroy(gameObject);
        }

        public void Init(ISimulation simulation)
        {
            var bedSystem = simulation.GetSystem<IGardenBedSystem>();
            var list = new List<GardenBedView>(gardenBeds);
            list.Sort((a, b) =>
            {
                return a.transform.position.sqrMagnitude.CompareTo(b.transform.position.sqrMagnitude);
            });
            foreach (GardenBedBehaviour bedBehaviour in bedSystem.GetBeds())
            {
                var bedView = list[bedBehaviour.Data.Number];
                bedView.Init(bedBehaviour);
            }
        }
    }
}