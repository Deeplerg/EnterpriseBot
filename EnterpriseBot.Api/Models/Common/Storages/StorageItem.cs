using EnterpriseBot.Api.Models.Common.Crafting;

namespace EnterpriseBot.Api.Models.Common.Storages
{
    public class StorageItem
    {
        public long Id { get; set; }

        public long ItemId { get; set; }
        public virtual Item Item { get; set; }

        public int Quantity { get; set; }
    }
}
