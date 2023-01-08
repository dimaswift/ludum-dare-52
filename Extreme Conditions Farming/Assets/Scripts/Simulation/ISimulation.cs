using System;
using ECF.Behaviours.Systems;
using ECF.Behaviours;
using ECF.Domain;
using ECF.Domain.Common;

namespace ECF.Behaviours
{
    public interface ISimulation
    {
        event Action OnGameOver;
        ObservableValue<int> Time { get; }
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
        IInventorySystem Inventory { get; }
        int GetRandom(int min, int max);
        SimulationState State { get; }
        void CreateSystems();
    }
}