using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Storages
{
    public class ShowcaseStorageCreationParams
    {
        public decimal Capacity { get; set; }

        public long OwningCompanyId { get; set; }
    }
}
