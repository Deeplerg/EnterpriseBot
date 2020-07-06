using EnterpriseBot.Api.Models.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Storages
{
    public class CompanyStorageApiCreationParams
    {
        public decimal Capacity { get; set; }

        public long OwningCompanyId { get; set; }

        public CompanyStorageType Type { get; set; }
    }
}
