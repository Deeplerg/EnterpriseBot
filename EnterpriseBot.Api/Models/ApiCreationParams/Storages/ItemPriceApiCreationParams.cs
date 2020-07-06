using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Storages
{
    public class ItemPriceApiCreationParams
    {
        public decimal Price { get; set; }
        public long ItemId { get; set; }
        public long OwningShowcaseStorageId { get; set; }
    }
}
