namespace ECF.Simulation.Systems
{
    public abstract class BaseSystem : ISystem
    {
        public virtual void OnInit(int time)
        {
            
        }

        public virtual void OnDispose()
        {
           
        }

        public virtual void OnTick(int time, int delta)
        {
            
        }

        public virtual void SaveState()
        {
            
        }
    }
}