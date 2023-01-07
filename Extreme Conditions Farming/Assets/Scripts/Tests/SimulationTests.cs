using System.Collections.Generic;
using ECF.Behaviours;
using ECF.Behaviours.Systems;
using ECF.Domain;
using NUnit.Framework;

public class SimulationTests
{
    [Test]
    public void SimulationTestPasses()
    {
        var items = new HashSet<MockSimulated>();
        var simulation = new Simulation();
        simulation.OnRemoved += s => { items.Remove(s as MockSimulated); };
        simulation.OnAdded += s => { items.Add(s as MockSimulated); };
        var delta = 10;
        simulation.Tick(delta);
        Assert.AreEqual(delta, simulation.Time.Value);
        simulation.Tick(delta);
        Assert.AreEqual(delta * 2, simulation.Time.Value);
        var simulated = new MockSimulated();
        simulation.Add(simulated);
        Assert.IsFalse(simulation.IsSimulated(simulated));
        Assert.AreEqual(0, items.Count);
        simulation.Tick(delta);
        Assert.AreEqual(simulated.ActualSpawnTime, simulation.Time.Value - delta);
        Assert.AreEqual(simulated.ActualSpawnTime, simulation.GetSpawnTime(simulated));

        Assert.AreEqual(simulated.ActualLifetime, delta);
        Assert.AreEqual(simulated.ActualLifetime, simulation.GetLifetime(simulated));
        
        Assert.IsTrue(simulation.IsSimulated(simulated));
        Assert.AreEqual(1, items.Count);
        Assert.AreEqual(delta, simulation.GetLifetime(simulated));
        simulation.Tick(delta);
        Assert.AreEqual(delta * 2, simulation.GetLifetime(simulated));
        simulation.Remove(simulated);
        Assert.AreEqual(1, items.Count);
        Assert.IsFalse(simulated.Disposed);
        simulation.Tick(delta);
        Assert.AreEqual(0, items.Count);
        Assert.IsTrue(simulated.Disposed);
        Assert.IsFalse(simulation.IsSimulated(simulated));
    }

    private class MockSimulated : ISimulated
    {
        public bool Disposed { get; private set; }
        public int ActualLifetime { get; private set; }
        public int ActualSpawnTime { get; private set; }
        public void OnInit(int time)
        {
            ActualSpawnTime = time;
        }

        public void OnDispose()
        {
            Disposed = true;
        }

        public void OnTick(int time, int delta)
        {
            ActualLifetime += delta;
        }
    }
}
