using EnterpriseBot.ApiWrapper.Models.Game.Business.Company;
using System.Collections.Generic;

namespace EnterpriseBot.ApiWrapper.Models.Game.Storages
{
    public class TrunkStorage
    {
        public long Id { get; set; }

        public Truck OwningTruck { get; set; }

        public long StorageId { get; set; }

        public decimal Capacity { get; set; }
        public decimal AvailableSpace { get; set; }
        public decimal OccupiedSpace { get; set; }
        public List<StorageItem> Items { get; set; } = new List<StorageItem>();
    }
}
