using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Storages
{
    public class TrunkStorageCreationParams
    {
        public decimal Capacity { get; set; }

        public long OwningTruckId { get; set; }
    }
}
