using EnterpriseBot.ApiWrapper.Models.Enums;
using EnterpriseBot.ApiWrapper.Models.Game.Crafting;

namespace EnterpriseBot.ApiWrapper.Models.Game.Business.Company
{
    public class CompanyContractRequest
    {
        public long Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public Company RequestedCompany { get; set; }
        public Company RequestingCompany { get; set; }
        public CompanyContractIssuer RequestingCompanyRelationSide { get; set; }

        public Item ContractItem { get; set; }
        public int ContractItemQuantity { get; set; }

        public decimal ContractOverallCost { get; set; }
        public sbyte TerminationTermInDays { get; set; }
    }
}
