using EnterpriseBot.ApiWrapper.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Business.Company
{
    public class CompanyContractRequestCreationParams
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public long RequestedCompanyId { get; set; }
        public long RequestingCompanyId { get; set; }
        public CompanyContractIssuer RequestingCompanyRelationSide { get; set; }

        public long ItemId { get; set; }
        public int ItemQuantity { get; set; }

        public decimal OverallCost { get; set; }
        public sbyte TerminationTermInDays { get; set; }
    }
}
