using EnterpriseBot.Api.Models.Common.Crafting;
using EnterpriseBot.Api.Models.Common.Enums;

namespace EnterpriseBot.Api.Models.Common.Business
{
    public class ContractInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


        public BusinessType ContractIncomeBusinessType { get; set; }

        public long OutcomeCompanyId { get; set; }
        public virtual Company OutcomeCompany { get; set; }

        public long? IncomeCompanyId { get; set; }
        public virtual Company IncomeCompany { get; set; }

        public long? IncomeShopId { get; set; }
        public virtual Shop IncomeShop { get; set; }


        public long ContractItemId { get; set; }
        public virtual Item ContractItem { get; set; }

        public sbyte TerminationTermInWeeks { get; set; }

        public uint ContractItemQuantity { get; set; } //quantity of items to be delivered every week
        public decimal ContractOverallCost { get; set; } //how much units income company/shop would pay to outcome company as the contract starts
    }
}
