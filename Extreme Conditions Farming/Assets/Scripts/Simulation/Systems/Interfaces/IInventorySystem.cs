using System;
using System.Collections.Generic;
using ECF.Domain;
using ECF.Domain.Common;

namespace ECF.Simulation.Systems
{
    public interface IInventorySystem
    {
        event Action<InventoryItem> OnItemAdded;
        event Action<InventoryItem> OnItemUsed;
        void Add(string id, int amount);
        bool Use(string id, int amount);
        
        IEnumerable<InventoryItem> GetItems();
        int Get(string id);
        void Save();
    }
}