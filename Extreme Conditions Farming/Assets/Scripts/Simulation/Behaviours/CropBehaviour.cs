using ECF.Domain;
using ECF.Domain.Common;

namespace ECF.Simulation.Behaviours
{
    public interface ICropBehaviour
    {
        IObservableValue<CropPhase> Phase { get; }
    }
    
    public class CropBehaviour : ISimulated, ICropBehaviour
    {
        public IObservableValue<CropPhase> Phase => phase;
        private readonly CropTemplate template;
        private readonly ObservableValue<CropPhase> phase;
        private int currentGrowthProgress;
        private int nextPhaseProgress;
        
        public CropBehaviour(CropTemplate template, CropPhase startPhase = CropPhase.Seed)
        {
            this.template = template;
            phase = new ObservableValue<CropPhase>(startPhase);
            nextPhaseProgress = this.template.PhaseStats.Durations[phase.Value];
        }
        
        public void OnInit(int time)
        {
            
        }

        public void OnDispose()
        {
            
        }

        public void OnTick(int time, int delta)
        {
            currentGrowthProgress += delta;
            while (currentGrowthProgress >= nextPhaseProgress)
            {
                currentGrowthProgress -= nextPhaseProgress;
               
                if (phase.Value == CropPhase.Rotten)
                {
                    break;
                }

                phase.Value = (CropPhase)((int)phase.Value + 1);
                nextPhaseProgress = template.PhaseStats.Durations[phase.Value];
            }
        }
    }
}