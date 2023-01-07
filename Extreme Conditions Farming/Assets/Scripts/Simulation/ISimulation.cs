using System;

namespace ECF.Simulation
{
    public interface ISimulation
    {
        int Time { get; }
        void Tick(int delta);
        void Spawn(ISimulated simulated);
        void Dispose(ISimulated simulated);
        bool IsSimulated(ISimulated simulated);
        event Action<ISimulated> OnSpawned;
        event Action<ISimulated> OnDisposed;
        int GetLifetime(ISimulated simulated);
        int GetSpawnTime(ISimulated simulated);
    }
}