using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Storages
{
    public class ItemPriceCreationParams
    {
        public decimal Price { get; set; }
        public long ItemId { get; set; }
        public long OwningShowcaseStorageId { get; set; }
    }
}
