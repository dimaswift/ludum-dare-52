using System;
using System.Collections.Generic;
using ECF.Behaviours.Systems;
using ECF.Domain;
using ECF.Domain.Common;
using UnityEngine;
using Random = System.Random;

namespace ECF.Behaviours
{
    public class Simulation : ISimulation
    {
        public SimulationState State => state;
        public ICropTemplateFactory CropTemplateFactory { get; }
        public IInventorySystem Inventory { get; }
        public event Action<ISimulated> OnAdded;
        public event Action<ISimulated> OnRemoved;
        private readonly HashSet<ISimulated> simulatedObjects = new();
        private readonly Queue<ISimulated> addQueue = new();
        private readonly Queue<ISimulated> removeQueue = new();
        private readonly Dictionary<Type, ISystem> systems = new();
        private readonly Dictionary<ISimulated, SimulatedTime> lifeTimes = new();
        private readonly Random random;
        private readonly SimulationState state;
        
        private struct SimulatedTime
        {
            public int Time { get; }
            public int SpawnTime { get; }

            public SimulatedTime(int time, int spawnTime)
            {
                Time = time;
                SpawnTime = spawnTime;
            }
        }

        public int GetRandom(int min, int max)
        {
            return random.Next(min, max);
        }



        public Simulation(SimulationState state = null)
        {
            if (state == null)
            {
                state = new SimulationState()
                {
                    RandomSeed = 0,
                    Time = 0,
                    Inventory = new InventorySystemData()
                    {
                        Items = new List<InventoryItemData>()
                    },
                    CropStorage = new CropStorageData()
                    {
                        Crops = new List<Crop>()
                    },
                    GardenBeds = new GardenBedSystemData()
                    {
                        Beds = new List<GardenBed>()
                    },
                };

                for (int i = 0; i < 16; i++)
                {
                    state.GardenBeds.Beds.Add(new GardenBed()
                    {
                        Status = BedStatus.Empty,
                        Number = i,
                        Crop = null,
                    });
                }
                
            }
            this.state = state;
            Time.Value = this.state.Time;
            Inventory = new InventorySystem(state.Inventory);
            AddSystem<ICropStorage>(new CropStorage(this));
            AddSystem<IGardenBedSystem>(new GardenBedSystem(this));
            CropTemplateFactory = new CropTemplateFactory();
            random = new Random(this.state.RandomSeed);
        }

        public ObservableValue<int> Time { get; } = new(0);

        private void ProcessSpawned()
        {
            while (addQueue.TryDequeue(out var spawned))
            {
                spawned.OnInit(Time.Value);
                if (simulatedObjects.Contains(spawned))
                {
                    continue;
                }

                simulatedObjects.Add(spawned);
                lifeTimes.Add(spawned, new SimulatedTime(0, Time.Value));
                OnAdded?.Invoke(spawned);
            }
        }

        private void ProcessDisposed()
        {
            while (removeQueue.TryDequeue(out var disposed))
            {
                disposed.OnDispose();
                if (simulatedObjects.Contains(disposed))
                {
                    simulatedObjects.Remove(disposed);
                    lifeTimes.Remove(disposed);
                }

                OnRemoved?.Invoke(disposed);
            }
        }

        private void ProcessSimulated(int delta)
        {
            foreach (ISimulated simulated in simulatedObjects)
            {
                simulated.OnTick(Time.Value, delta);
                var lifetime = lifeTimes[simulated];
                lifeTimes[simulated] = new SimulatedTime(lifetime.Time + delta, lifetime.SpawnTime);
            }
        }

        private void ProcessSystems(int delta)
        {
            foreach (var system in systems)
            {
                system.Value.OnTick(Time.Value, delta);
            }
        }
        
        public void Tick(int delta)
        {
            ProcessSpawned();

            ProcessSimulated(delta);
            
            ProcessSystems(delta);
            
            ProcessDisposed();

            Time.Value += delta;
        }

        public void Add(ISimulated simulated)
        {
            addQueue.Enqueue(simulated);
        }

        public void Remove(ISimulated simulated)
        {
            removeQueue.Enqueue(simulated);
        }

        public bool IsSimulated(ISimulated simulated)
        {
            return simulatedObjects.Contains(simulated);
        }

        public int GetLifetime(ISimulated simulated)
        {
            if (lifeTimes.TryGetValue(simulated, out var lifetime))
            {
                return lifetime.Time;
            }
            return -1;
        }

        public int GetSpawnTime(ISimulated simulated)
        {
            if (lifeTimes.TryGetValue(simulated, out var lifetime))
            {
                return lifetime.SpawnTime;
            }
            return -1;
        }

        public void AddSystem<T>(T system) where T : ISystem
        {
            systems.Add(typeof(T), system);
        }

        public T GetSystem<T>() where T : ISystem
        {
            return (T) systems[typeof(T)];
        }

        public void SaveState()
        {
            foreach (var system in systems)
            {
                system.Value.SaveState();
            }
            
            state.Time = Time.Value;
        }
    }
}