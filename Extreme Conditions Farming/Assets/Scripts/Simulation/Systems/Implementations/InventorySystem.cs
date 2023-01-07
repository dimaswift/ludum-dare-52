using System;
using System.Collections.Generic;
using System.Linq;
using ECF.Domain;
using ECF.Domain.Common;

namespace ECF.Simulation.Systems
{
    public class InventorySystem : IInventorySystem
    {
        public event Action<InventoryItem> OnItemAdded;
        public event Action<InventoryItem> OnItemUsed;

        private readonly SaveData data;
        
        private readonly Dictionary<string, InventoryItem> items = new();

        private readonly IStorageService storageService;
        
        public class ItemData
        {
            public int Amount { get; set; }
            public string Id { get; set; }
        }
        
        private class SaveData
        {
            public List<ItemData> Items { get; set; }
        }

        private SaveData GetDefaultStorage()
        {
            return new SaveData()
            {
                Items = new List<ItemData>()
            };
        }
        
        public InventorySystem(IStorageService storageService)
        {
            this.storageService = storageService;
            data = storageService.Load(GetDefaultStorage);
            foreach (ItemData data in data.Items)
            {
                items.Add(data.Id, new InventoryItem()
                {
                    Id = data.Id,
                    Amount = new ObservableValue<int>(data.Amount)
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
            data.Items = items.Select(item => new ItemData
            {
                Amount = item.Value.Amount.Value,
                Id = item.Value.Id
            }).ToList();
            storageService.Save(data);
        }
    }
}