using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Business
{
    public class TruckGarageApiCreationParams
    {
        public long OwningCompanyId { get; set; }
        public sbyte Capacity { get; set; }
    }
}
