using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Models.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Business
{
    public class CompanyContractCreationParams
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Company IncomeCompany { get; set; }

        public Company OutcomeCompany { get; set; }

        public CompanyContractIssuer Issuer { get; set; }

        public Item Item { get; set; }
        public int ItemQuantity { get; set; }
        public decimal OverallCost { get; set; }
        public sbyte TerminationTermInDays { get; set; }
    }
}
