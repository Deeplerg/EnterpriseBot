using EnterpriseBot.Api.Models.Common.Enums;

namespace EnterpriseBot.Api.Models.ApiCreationParams.Business
{
    public class CompanyContractRequestApiCreationParams
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
