using System;
using System.Collections.Generic;
using System.Linq;
using ECF.Domain;
using ECF.Domain.Common;

namespace ECF.Behaviours.Systems
{
    public class InventorySystem : IInventorySystem
    {
        public event Action<InventoryItem> OnItemAdded;
        public event Action<InventoryItem> OnItemUsed;

        private readonly InventorySystemData data;
        
        private readonly Dictionary<string, InventoryItem> items = new();
        
        public InventorySystem(InventorySystemData data)
        {
            this.data = data;
            foreach (InventoryItemData d in data.Items)
            {
                items.Add(d.Id, new InventoryItem()
                {
                    Id = d.Id,
                    Amount = new ObservableValue<int>(d.Amount)
                });
            }
        }
        
        public void Add(string id, int amount)
        {
            if (items.TryGetValue(id, out var item))
            {
                item.Amount.Value += amount;
            }
            else
            {
                item = new InventoryItem()
                {
                    Id = id,
                    Amount = new ObservableValue<int>(amount)
                };
                items.Add(id, item);
                OnItemAdded?.Invoke(item);
            }
        }
        
        public bool Use(string id, int amount)
        {
            if (!items.TryGetValue(id, out InventoryItem item) || item.Amount.Value < amount)
            {
                return false;
            }
            item.Amount.Value -= amount;
            OnItemUsed?.Invoke(item);
            return true;
        }

        public IEnumerable<InventoryItem> GetItems()
        {
            return items.Values;
        }

        public int Get(string test)
        {
            if (items.TryGetValue(test, out InventoryItem item))
            {
                return item.Amount.Value;
            }
            return 0;
        }

        public void Save()
        {
            data.Items = items.Select(item => new InventoryItemData
            {
                Amount = item.Value.Amount.Value,
                Id = item.Value.Id
            }).ToList();
        }
    }
}