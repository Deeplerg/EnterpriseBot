using EnterpriseBot.ApiWrapper.Models.Game.Essences;
using System.Collections.Generic;

namespace EnterpriseBot.ApiWrapper.Models.Game.Storages
{
    public class InventoryStorage
    {
        public long Id { get; set; }

        public Player OwningPlayer { get; set; }

        public long StorageId { get; set; }

        public decimal Capacity { get; set; }
        public decimal AvailableSpace { get; set; }
        public decimal OccupiedSpace { get; set; }
        public List<StorageItem> Items { get; set; } = new List<StorageItem>();
    }
}
