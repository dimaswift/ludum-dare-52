using System.Collections.Generic;
using ECF.Domain;
using ECF.Simulation.Behaviours;

namespace ECF.Simulation.Systems
{
    public class CropSystem : ICropSystem
    {
        private readonly ISimulation simulation;
        
        private readonly HashSet<CropBehaviour> crops = new();

        public CropSystem(ISimulation simulation)
        {
            this.simulation = simulation;
        }
        
        public void OnInit(int time)
        {
            
        }

        public void OnDispose()
        {
            
        }

        public void OnTick(int time, int delta)
        {
            
        }

        public void Save()
        {
            
        }

        public CropBehaviour PlantCrop(CropTemplate template)
        {
            var crop = new CropBehaviour(template);

            crops.Add(crop);
            simulation.Add(crop);
            
            return crop;
        }
    }
}