using EnterpriseBot.ApiWrapper.Models.Game.Business.Company;
using EnterpriseBot.ApiWrapper.Models.Enums;
using System.Collections.Generic;

namespace EnterpriseBot.ApiWrapper.Models.Game.Storages
{
    public class CompanyStorage
    {
        public long Id { get; set; }

        public Company OwningCompany { get; set; }
        
        public CompanyStorageType Type { get; set; }

        public long StorageId { get; set; }

        public decimal Capacity { get; set; }
        public decimal AvailableSpace { get; set; }
        public decimal OccupiedSpace { get; set; }
        public List<StorageItem> Items { get; set; } = new List<StorageItem>();
    }
}
