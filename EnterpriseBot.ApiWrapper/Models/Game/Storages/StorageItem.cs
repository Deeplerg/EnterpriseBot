using EnterpriseBot.ApiWrapper.Models.Game.Crafting;

namespace EnterpriseBot.ApiWrapper.Models.Game.Storages
{
    public class StorageItem
    {
        public long Id { get; set; }

        public Item Item { get; set; }

        public int Quantity { get; set; }

        /// <summary>
        /// How much space it takes or would take in a storage
        /// </summary>
        public decimal Space { get; set; }
    }
}
