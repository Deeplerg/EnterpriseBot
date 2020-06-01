using EnterpriseBot.Api.Models.Common.Crafting;
using EnterpriseBot.Api.Models.Common.Enums;
using Newtonsoft.Json;
using System;

namespace EnterpriseBot.Api.Models.Common.Business
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

        public uint DeliveredAmount { get; set; } //how many items are already delivered
        public uint ContractItemQuantity { get; set; } //items amount to be delivered every week
        public decimal ContractOverallCost { get; set; }

        [JsonIgnore]
        public string CompletionCheckerBackgroundJobId { get; set; }
        [JsonIgnore]
        public string BreakerBackgroundJobId { get; set; }
    }
}
