namespace EnterpriseBot.ApiWrapper.Models.Common.Storages
{
    public class StorageItemWithPrice
    {
        public long Id { get; set; }

        //public long ItemId { get; set; }
        //public Item Item { get; set; }

        //public int Quantity { get; set; }

        public long ShowcaseStorageId { get; set; }
        public virtual ShowcaseStorage ShowcaseStorage { get; set; }

        public virtual StorageItem StorageItem { get; set; }

        public decimal Price { get; set; }
    }
}
