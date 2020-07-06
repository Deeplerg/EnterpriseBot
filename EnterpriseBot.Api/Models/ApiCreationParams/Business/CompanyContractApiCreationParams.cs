using EnterpriseBot.Api.Models.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Business
{
    public class CompanyContractApiCreationParams
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long IncomeCompanyId { get; set; }
        public long OutcomeCompanyId { get; set; }
        public CompanyContractIssuer Issuer { get; set; }
        public long ItemId { get; set; }
        public int ItemQuantity { get; set; }
        public decimal OverallCost { get; set; }
        public sbyte TerminationTermInWeeks { get; set; }
    }
}
