using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Models.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Business
{
    public class ContractCreationParams
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Company IncomeCompany { get; set; }

        public Company OutcomeCompany { get; set; }

        public CompanyContractIssuer Issuer { get; set; }

        public Item ContractItem { get; set; }
        public int ContractItemQuantity { get; set; }
        public decimal ContractOverallCost { get; set; }
        public sbyte TerminationTermInWeeks { get; set; }
    }
}
