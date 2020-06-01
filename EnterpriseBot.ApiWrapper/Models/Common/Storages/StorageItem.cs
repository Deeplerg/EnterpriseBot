using EnterpriseBot.ApiWrapper.Models.Common.Crafting;

namespace EnterpriseBot.ApiWrapper.Models.Common.Storages
{
    public class StorageItem
    {
        public long Id { get; set; }

        public long ItemId { get; set; }
        public virtual Item Item { get; set; }

        public int Quantity { get; set; }
    }
}
