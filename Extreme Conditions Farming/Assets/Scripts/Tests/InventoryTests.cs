using System.Linq;
using ECF.Simulation;
using ECF.Simulation.Systems;
using NUnit.Framework;

public class InventoryTests
{
    [Test]
    public void InventoryTestsPasses()
    {
        var saveSystem = new MockStorage();
        var inventory = new InventorySystem(saveSystem);
        var simulation = new Simulation(saveSystem, inventory);

        inventory.Add("test", 1);
        Assert.AreEqual(1, inventory.Get("test"));
        
        inventory.Add("test", 1);
        Assert.AreEqual(2, inventory.Get("test"));
        
        var used = inventory.Use("test", 1);
        
        Assert.IsTrue(used);
        
        Assert.AreEqual(1, inventory.Get("test"));
        
        used = inventory.Use("test", 1);
        
        Assert.IsTrue(used);
        
        Assert.AreEqual(0, inventory.Get("test"));
        
        used = inventory.Use("test", 1);
        
        Assert.IsFalse(used);
        
        Assert.AreEqual(0, inventory.Get("test"));
        
        inventory.Add("test", 1);
        
        Assert.AreEqual(1, inventory.Get("test"));
        
        simulation.SaveState();
        
        simulation = new Simulation(saveSystem, inventory);

        Assert.AreEqual(1, inventory.Get("test"));

        var added = false;
        
        inventory.OnItemAdded += i => { added = true; };
        
        inventory.Add("test2", 100);
        
        Assert.IsTrue(added);

        Assert.AreEqual(100, inventory.Get("test2"));

        Assert.AreEqual(1, inventory.Get("test"));

        bool itemUsed = false;

        inventory.OnItemUsed += i =>
        {
            itemUsed = true;
        };

        inventory.Use("test2", 10);
        
        Assert.IsTrue(itemUsed);
        
        Assert.AreEqual(90, inventory.Get("test2"));
        
        Assert.AreEqual(inventory.GetItems().Count(), 2);
        
        Assert.AreEqual(inventory.GetItems().First().Id, "test");
        
        Assert.AreEqual(inventory.GetItems().Last().Id, "test2");
        
        Assert.AreEqual(inventory.GetItems().First().Amount.Value, 1);
        
    }
 
}