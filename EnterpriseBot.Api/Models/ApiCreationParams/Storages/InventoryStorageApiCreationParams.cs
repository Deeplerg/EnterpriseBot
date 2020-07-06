using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Storages
{
    public class InventoryStorageApiCreationParams
    {
        public long OwningPlayerId { get; set; }
        public decimal Capacity { get; set; }
    }
}
