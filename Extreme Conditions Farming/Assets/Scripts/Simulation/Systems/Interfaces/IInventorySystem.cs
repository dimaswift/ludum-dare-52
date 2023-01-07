using System;
using System.Collections.Generic;
using ECF.Domain;
using ECF.Domain.Common;

namespace ECF.Simulation.Systems
{
    public interface IInventorySystem : ISystem
    {
        event Action<InventoryItem> OnItemAdded;
        event Action<InventoryItem> OnItemUsed;
        void Add(string id, int amount);
        bool Use(string id);
        IEnumerable<InventoryItem> GetItems();
    }
}