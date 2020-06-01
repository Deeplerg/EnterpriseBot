using EnterpriseBot.Api.Models.Common.Enums;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Business
{
    public class ContractRequestCreationParams
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public BusinessType IncomeBusinessType { get; set; }
        public long IncomeBusinessId { get; set; }

        public long OutcomeCompanyId { get; set; }

        public long ContractItemId { get; set; }
        public uint ContractItemQuantity { get; set; }
        public decimal ContractOverallCost { get; set; }
        public sbyte TerminationTermInWeeks { get; set; }
    }
}
