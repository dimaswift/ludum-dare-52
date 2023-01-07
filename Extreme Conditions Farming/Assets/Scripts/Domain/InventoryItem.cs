using ECF.Domain.Common;

namespace ECF.Domain
{
    public class InventoryItem
    {
        public string Id { get; set; }
        public IObservableValue<int> Amount { get; set; }
    }
}