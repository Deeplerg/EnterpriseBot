using EnterpriseBot.ApiWrapper.Models.Enums;
using EnterpriseBot.ApiWrapper.Models.Game.Crafting;
using System;

namespace EnterpriseBot.ApiWrapper.Models.Game.Business.Company
{
    public class CompanyContract
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


        public Company OutcomeCompany { get; set; }
        public Company IncomeCompany { get; set; }
        public CompanyContractIssuer Issuer { get; set; }

        public DateTimeOffset ConclusionDate { get; set; }
        public sbyte TerminationTermInDays { get; set; }

        public Item ContractItem { get; set; }

        public int DeliveredAmount { get; set; } //how many items are already delivered
        public int ContractItemQuantity { get; set; } //items amount to be delivered
        public decimal ContractOverallCost { get; set; }

        public bool IsCompleted { get; set; }
    }
}
