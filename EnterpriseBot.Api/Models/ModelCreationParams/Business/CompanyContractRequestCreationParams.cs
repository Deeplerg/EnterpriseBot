using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Crafting;
using EnterpriseBot.Api.Models.Common.Enums;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Business
{
    public class CompanyContractRequestCreationParams
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Company RequestedCompany { get; set; }
        public Company RequestingCompany { get; set; }
        public CompanyContractIssuer RequestingCompanyRelationSide { get; set; }

        public Item Item { get; set; }
        public int ItemQuantity { get; set; }

        public decimal OverallCost { get; set; }
        public sbyte TerminationTermInDays { get; set; }
    }
}
