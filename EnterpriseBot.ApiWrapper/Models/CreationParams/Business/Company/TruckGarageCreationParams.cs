using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Business.Company
{
    public class TruckGarageCreationParams
    {
        public long OwningCompanyId { get; set; }
        public sbyte Capacity { get; set; }
    }
}
