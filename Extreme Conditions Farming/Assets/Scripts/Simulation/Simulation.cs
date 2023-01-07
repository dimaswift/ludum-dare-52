using System;
using System.Collections.Generic;

namespace ECF.Simulation
{
    public class Simulation : ISimulation
    {
        public event Action<ISimulated> OnAdded;
        public event Action<ISimulated> OnRemoved;
        private readonly HashSet<ISimulated> simulatedObjects = new();
        private readonly Queue<ISimulated> addQueue = new();
        private readonly Queue<ISimulated> removeQueue = new();
        private readonly Dictionary<Type, ISystem> systems = new();

        private readonly Dictionary<ISimulated, SimulatedTime> lifeTimes = new();
        
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

        public int Time => time;

        private int time;

        private void ProcessSpawned()
        {
            while (addQueue.TryDequeue(out var spawned))
            {
                spawned.OnInit(time);
                if (simulatedObjects.Contains(spawned))
                {
                    continue;
                }

                simulatedObjects.Add(spawned);
                lifeTimes.Add(spawned, new SimulatedTime(0, time));
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
                simulated.OnTick(time, delta);
                var lifetime = lifeTimes[simulated];
                lifeTimes[simulated] = new SimulatedTime(lifetime.Time + delta, lifetime.SpawnTime);
            }
        }

        private void ProcessSystems(int delta)
        {
            foreach (var system in systems)
            {
                system.Value.OnTick(time, delta);
            }
        }
        
        public void Tick(int delta)
        {
            ProcessSpawned();

            ProcessSimulated(delta);
            
            ProcessSystems(delta);
            
            ProcessDisposed();

            time += delta;
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

        public void SaveSystems()
        {
            foreach (var system in systems)
            {
                system.Value.Save();
            }
        }
    }
}