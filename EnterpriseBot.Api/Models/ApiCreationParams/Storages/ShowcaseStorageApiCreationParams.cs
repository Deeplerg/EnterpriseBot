using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Storages
{
    public class ShowcaseStorageApiCreationParams
    {
        public decimal Capacity { get; set; }

        public long OwningCompanyId { get; set; }
    }
}
