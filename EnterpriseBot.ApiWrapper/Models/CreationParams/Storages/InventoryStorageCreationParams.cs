using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Storages
{
    public class InventoryStorageCreationParams
    {
        public long OwningPlayerId { get; set; }
        public decimal Capacity { get; set; }
    }
}
