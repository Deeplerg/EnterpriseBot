using EnterpriseBot.ApiWrapper.Models.Common.Business;
using System.Collections.Generic;

namespace EnterpriseBot.ApiWrapper.Models.Common.Storages
{
    public class TrunkStorage
    {
        public long Id { get; set; }

        //public int CellsAmount { get; set; }
        //public int CellsCapacity { get; set; }
        public int Capacity { get; set; }

        public long TruckId { get; set; }
        public virtual Truck Truck { get; set; }

        public virtual List<StorageItem> Items { get; set; }
    }
}
