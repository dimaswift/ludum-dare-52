using System;
using ECF.Simulation.Systems;

namespace ECF.Simulation
{
    public interface ISimulation
    {
        int Time { get; }
        void Tick(int delta);
        void Add(ISimulated simulated);
        void Remove(ISimulated simulated);
        bool IsSimulated(ISimulated simulated);
        event Action<ISimulated> OnAdded;
        event Action<ISimulated> OnRemoved;
        int GetLifetime(ISimulated simulated);
        int GetSpawnTime(ISimulated simulated);
        void AddSystem<T>(T system) where T : ISystem;
        T GetSystem<T>() where T : ISystem;
        void SaveState();
        ICropTemplateFactory CropTemplateFactory { get; }
        IStorageService Storage { get; }
        IInventorySystem Inventory { get; }
    }

    public interface ISystem : ISimulated
    {
        void SaveState();
    }
}