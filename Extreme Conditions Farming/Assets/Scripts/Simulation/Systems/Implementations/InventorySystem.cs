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

        private readonly Storage storage;
        private readonly Dictionary<string, InventoryItem> items = new();

        private readonly ISaveSystem saveSystem;
        
        public class ItemData
        {
            public int Amount { get; set; }
            public string Id { get; set; }
        }
        
        private class Storage
        {
            public List<ItemData> Items { get; set; }
        }

        private Storage GetDefaultStorage()
        {
            return new Storage()
            {
                Items = new List<ItemData>()
            };
        }
        
        public InventorySystem(ISaveSystem saveSystem)
        {
            this.saveSystem = saveSystem;
            storage = saveSystem.Load(GetDefaultStorage);
            foreach (ItemData data in storage.Items)
            {
                items.Add(data.Id, new InventoryItem()
                {
                    Id = data.Id,
                    Amount = new ObservableValue<int>(data.Amount)
                });
            }
        }
        
        public void OnInit(int time)
        {
            
        }

        public void OnDispose()
        {
            
        }

        public void OnTick(int time, int delta)
        {
            
        }

        public void Save()
        {
            storage.Items = items.Select(item => new ItemData
            {
                Amount = item.Value.Amount.Value,
                Id = item.Value.Id
            }).ToList();
            saveSystem.Save(storage);
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

        public bool Use(string id)
        {
            if (!items.TryGetValue(id, out InventoryItem item) || item.Amount.Value <= 0)
            {
                return false;
            }
            item.Amount.Value--;
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
    }
}