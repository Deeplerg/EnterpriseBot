using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Storages
{
    public class TrunkStorageApiCreationParams
    {
        public decimal Capacity { get; set; }

        public long OwningTruckId { get; set; }
    }
}
