using EnterpriseBot.ApiWrapper.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.ApiWrapper.Models.CreationParams.Business.Company
{
    public class CompanyContractCreationParams
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long IncomeCompanyId { get; set; }
        public long OutcomeCompanyId { get; set; }
        public CompanyContractIssuer Issuer { get; set; }
        public long ItemId { get; set; }
        public int ItemQuantity { get; set; }
        public decimal OverallCost { get; set; }
        public sbyte TerminationTermInDays { get; set; }
    }
}
