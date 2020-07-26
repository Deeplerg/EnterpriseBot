using EnterpriseBot.ApiWrapper.Models.Game.Business.Company;
using System.Collections.Generic;

namespace EnterpriseBot.ApiWrapper.Models.Game.Storages
{
    public class ShowcaseStorage
    {
        public long Id { get; set; }

        public Company OwningCompany { get; set; }

        public List<ItemPrice> Prices { get; set; } = new List<ItemPrice>();

        public long StorageId { get; set; }

        public decimal Capacity { get; set; }
        public decimal AvailableSpace { get; set; }
        public decimal OccupiedSpace { get; set; }
        public List<StorageItem> Items { get; set; } = new List<StorageItem>();
    }
}
