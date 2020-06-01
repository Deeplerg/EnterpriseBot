using EnterpriseBot.ApiWrapper.Models.Common.Crafting;
using EnterpriseBot.ApiWrapper.Models.Common.Enums;
using System;

namespace EnterpriseBot.ApiWrapper.Models.Common.Business
{
    public class Contract
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

        public DateTimeOffset ConclusionDate { get; set; }
        public sbyte TerminationTermInWeeks { get; set; }

        //при создании контракта создается задача, каждую неделю проверяющая выполнение контракта.
        //если условия контракта не выполняются за неделю, он автоматически расторгается.
        public long ContractItemId { get; set; }
        public virtual Item ContractItem { get; set; }

        public uint DeliveredAmount { get; set; } //сколько уже доставлено
        public uint ContractItemQuantity { get; set; } //сколько нужно доставлять каждую неделю
        public decimal ContractOverallCost { get; set; }
    }
}
