using System;
using System.Collections.Generic;
using ECF.Domain;

namespace ECF.Behaviours.Systems
{
    public interface IInventorySystem
    {
        event Action<InventoryItem> OnItemAdded;
        event Action<InventoryItem> OnItemUsed;
        void Add(string id, int amount);
        bool Use(string id, int amount);
        IEnumerable<InventoryItem> GetItems();
        int Get(string id);
    }
}