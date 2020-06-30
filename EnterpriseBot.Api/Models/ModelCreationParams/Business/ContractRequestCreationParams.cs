using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Models.Common.Enums;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Business
{
    public class ContractRequestCreationParams
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Company RequestedCompany { get; set; }
        public Company RequestingCompany { get; set; }
        public CompanyContractIssuer RequestingCompanyRelationSide { get; set; }

        public Item ContractItem { get; set; }
        public int ContractItemQuantity { get; set; }

        public decimal ContractOverallCost { get; set; }
        public sbyte TerminationTermInWeeks { get; set; }
    }
}
