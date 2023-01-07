using System.Collections.Generic;
using ECF.Domain;
using ECF.Simulation;
using ECF.Simulation.Behaviours;
using ECF.Simulation.Systems;
using NUnit.Framework;

public class CropTests
{
    [Test]
    public void GardenBedBehaviourPhasesTestPasses()
    {
        var storage = new MockStorage();
        var simulation = new Simulation(storage, new InventorySystem(storage));
        var bed = new GardenBedBehaviour(simulation, new GardenBed()
        {
            Number = 0,
            Status = BedStatus.Empty,
        });
        var time = 0;
        BedStatus currentStatus = bed.Status.Value;
        var template = simulation.CropTemplateFactory.CreateLinear("Test", "Test", 10, 10, 10, 10);
        void Tick(int delta)
        {
            time += delta;
            bed.OnTick(time, delta);
        }
        
        Assert.AreEqual(BedStatus.Empty, currentStatus);
        
        var planted = bed.Plant(template, out _);
        
        Assert.IsFalse(planted);
        
        bed.Status.Changed += s =>
        {
            currentStatus = s;
        };
        
        Assert.AreEqual(BedStatus.Empty, currentStatus);
        
        simulation.Inventory.Add(template.SeedId, 1);
        
        planted = bed.Plant(template, out _);

        Assert.IsTrue(planted);
        
        Tick(template.PhaseStats.Durations[CropPhase.Seed]);

        Assert.AreEqual(bed.Status.Value, currentStatus);
        Assert.AreEqual(BedStatus.Planted, currentStatus);
        
        Assert.AreEqual(CropPhase.Sprout, bed.Phase.Value);
        
        Tick(template.PhaseStats.Durations[CropPhase.Sprout]);

        Assert.AreEqual(CropPhase.Bud, bed.Phase.Value);
        
        Tick(1000);

        Assert.AreEqual(CropPhase.Rotten, bed.Phase.Value);
    }

    [Test]
    public void CropSystemTestPasses()
    {
    
    }
}
