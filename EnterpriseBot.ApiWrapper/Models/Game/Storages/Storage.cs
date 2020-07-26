using System.Collections.Generic;

namespace EnterpriseBot.ApiWrapper.Models.Game.Storages
{
    public class Storage
    {
        public long Id { get; set; }

        public decimal Capacity { get; set; }

        public List<StorageItem> Items { get; set; } = new List<StorageItem>();

        public decimal AvailableSpace { get; set; }
        public decimal OccupiedSpace { get; set; }
    }
}
