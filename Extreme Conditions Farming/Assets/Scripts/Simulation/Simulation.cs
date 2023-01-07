using System;
using System.Collections.Generic;

namespace ECF.Simulation
{
    public class Simulation : ISimulation
    {
        public event Action<ISimulated> OnSpawned;
        public event Action<ISimulated> OnDisposed;
        private readonly HashSet<ISimulated> simulatedObjects = new();
        private readonly Queue<ISimulated> spawnQueue = new();
        private readonly Queue<ISimulated> disposeQueue = new();

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
            while (spawnQueue.TryDequeue(out var spawned))
            {
                spawned.OnInit(time);
                if (simulatedObjects.Contains(spawned))
                {
                    continue;
                }

                simulatedObjects.Add(spawned);
                lifeTimes.Add(spawned, new SimulatedTime(0, time));
                OnSpawned?.Invoke(spawned);
            }
        }

        private void ProcessDisposed()
        {
            while (disposeQueue.TryDequeue(out var disposed))
            {
                disposed.OnDispose();
                if (simulatedObjects.Contains(disposed))
                {
                    simulatedObjects.Remove(disposed);
                    lifeTimes.Remove(disposed);
                }

                OnDisposed?.Invoke(disposed);
            }
        }

        private void ProcessTick(int delta)
        {
            foreach (ISimulated simulated in simulatedObjects)
            {
                simulated.OnTick(time, delta);
                var lifetime = lifeTimes[simulated];
                lifeTimes[simulated] = new SimulatedTime(lifetime.Time + delta, lifetime.SpawnTime);
            }
        }
        
        public void Tick(int delta)
        {
            ProcessSpawned();

            ProcessTick(delta);
            
            ProcessDisposed();

            time += delta;
        }

        public void Spawn(ISimulated simulated)
        {
            spawnQueue.Enqueue(simulated);
        }

        public void Dispose(ISimulated simulated)
        {
            disposeQueue.Enqueue(simulated);
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
    }
}