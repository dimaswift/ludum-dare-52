using System.Collections.Generic;
using ECF.Domain;
using ECF.Simulation;
using ECF.Simulation.Behaviours;
using ECF.Simulation.Systems;
using NUnit.Framework;

public class CropTests
{
    [Test]
    public void CropBehaviourPhasesTestPasses()
    {
        var durations = new Dictionary<CropPhase, int>
        {
            { CropPhase.Seed, 2},
            { CropPhase.Sprout, 3},
            { CropPhase.Bud, 3},
            { CropPhase.Flower, 4},
            { CropPhase.Green, 5},
            { CropPhase.Unripe, 6},
            { CropPhase.Ripe, 7 },
            { CropPhase.Overripe, 8 },
            { CropPhase.Rotten, 6 }
        };

        var template = new CropTemplateBuilder()
            .WithName("TestCrop");

        foreach (var duration in durations)
        {
            template.WithPhaseDuration(duration.Key, duration.Value);
        }
        
        var crop = new CropBehaviour(template.Build());
        var time = 0;
        CropPhase currentPhase = crop.Phase.Value;

        void Tick(int delta)
        {
            time += delta;
            crop.OnTick(time, delta);
        }
        
        Assert.AreEqual(CropPhase.Seed, currentPhase);
        
        crop.Phase.Changed += phase =>
        {
            currentPhase = phase;
        };

        Tick(durations[CropPhase.Seed] - 1);

        Assert.AreEqual(crop.Phase.Value, currentPhase);
        Assert.AreEqual(CropPhase.Seed, currentPhase);
        
        Tick(1);

        Assert.AreEqual(crop.Phase.Value, currentPhase);
        Assert.AreEqual(CropPhase.Sprout, currentPhase);
        
        Tick(durations[CropPhase.Bud]);

        Assert.AreEqual(crop.Phase.Value, currentPhase);
        Assert.AreEqual(CropPhase.Bud, currentPhase);
        
        Tick(1000);
        
        Assert.AreEqual(crop.Phase.Value, currentPhase);
        Assert.AreEqual(CropPhase.Rotten, currentPhase);
    }

    [Test]
    public void CropSystemTestPasses()
    {
        var simulation = new Simulation();
        var system = new CropSystem(simulation);
        var template1 = CropTemplateBuilder.CreateLinear("Crop1", 10, 10, 10, 10);
        var template2 = CropTemplateBuilder.CreateLinear("Crop2", 20, 20, 20, 20);
        var crop1 = system.PlantCrop(template1);
        var crop2 = system.PlantCrop(template2);
    }
}
