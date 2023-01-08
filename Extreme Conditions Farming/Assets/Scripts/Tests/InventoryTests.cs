using System.Collections.Generic;
using System.Linq;
using ECF.Behaviours;
using ECF.Behaviours.Systems;
using ECF.Domain;
using NUnit.Framework;

public class InventoryTests
{
    [Test]
    public void InventoryTestsPasses()
    {
        var inventory = new InventorySystem(new InventorySystemData()
        {
            Items = new List<InventoryItemData>()
        });

        inventory.Add("test", 1);
        Assert.AreEqual(1, inventory.Get("test").Value);
        
        inventory.Add("test", 1);
        Assert.AreEqual(2, inventory.Get("test").Value);
        
        var used = inventory.Use("test", 1);
        
        Assert.IsTrue(used);
        
        Assert.AreEqual(1, inventory.Get("test").Value);
        
        used = inventory.Use("test", 1);
        
        Assert.IsTrue(used);
        
        Assert.AreEqual(0, inventory.Get("test").Value);
        
        used = inventory.Use("test", 1);
        
        Assert.IsFalse(used);
        
        Assert.AreEqual(0, inventory.Get("test").Value);
        
        inventory.Add("test", 1);
        
        Assert.AreEqual(1, inventory.Get("test").Value);
        
        Assert.AreEqual(1, inventory.Get("test").Value);

        var added = false;
        
        inventory.OnItemAdded += i => { added = true; };
        
        inventory.Add("test2", 100);
        
        Assert.IsTrue(added);

        Assert.AreEqual(100, inventory.Get("test2").Value);

        Assert.AreEqual(1, inventory.Get("test").Value);

        bool itemUsed = false;

        inventory.OnItemUsed += i =>
        {
            itemUsed = true;
        };

        inventory.Use("test2", 10);
        
        Assert.IsTrue(itemUsed);
        
        Assert.AreEqual(90, inventory.Get("test2").Value);
        
        Assert.AreEqual(inventory.GetItems().Count(), 2);
        
        Assert.AreEqual(inventory.GetItems().First().Id, "test");
        
        Assert.AreEqual(inventory.GetItems().Last().Id, "test2");
        
        Assert.AreEqual(inventory.GetItems().First().Amount.Value, 1);
        
    }
 
}