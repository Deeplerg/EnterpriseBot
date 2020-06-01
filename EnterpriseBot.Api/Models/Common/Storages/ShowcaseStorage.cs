using EnterpriseBot.Api.Models.Common.Business;
using System.Collections.Generic;

namespace EnterpriseBot.Api.Models.Common.Storages
{
    public class ShowcaseStorage// : IStorage
    {
        public long Id { get; set; }

        //public int CellsAmount { get; set; }
        //public int CellsCapacity { get; set; }
        public int Capacity { get; set; }

        public long OwningShopId { get; set; }
        public virtual Shop OwningShop { get; set; }


        public virtual List<StorageItemWithPrice> Items { get; set; }
    }
}
