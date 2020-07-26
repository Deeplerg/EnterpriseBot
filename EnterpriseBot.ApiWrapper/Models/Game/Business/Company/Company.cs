using EnterpriseBot.ApiWrapper.Models.Enums;
using EnterpriseBot.ApiWrapper.Models.Game.Essences;
using EnterpriseBot.ApiWrapper.Models.Game.Localization;
using EnterpriseBot.ApiWrapper.Models.Game.Money;
using EnterpriseBot.ApiWrapper.Models.Game.Storages;
using System.Collections.Generic;

namespace EnterpriseBot.ApiWrapper.Models.Game.Business.Company
{
    public class Company
    {
        public long Id { get; set; }

        public string Name { get; set; }
        public LocalizedString Description { get; set; }

        public Player Owner { get; set; }
        public Purse Purse { get; set; }
        public Reputation.Reputation Reputation { get; set; }

        public TruckGarage TruckGarage { get; set; }
        public uint ContractsCompleted { get; set; }
        public CompanyExtensions Extensions { get; set; }

        public List<CompanyJob> Jobs { get; set; } = new List<CompanyJob>();
        public List<ProductionRobot> Robots { get; set; } = new List<ProductionRobot>();
        public List<CompanyContract> IncomeContracts { get; set; } = new List<CompanyContract>();
        public List<CompanyContract> OutcomeContracts { get; set; } = new List<CompanyContract>();
        public List<CompanyContractRequest> SentContractRequests { get; set; } = new List<CompanyContractRequest>();
        public List<CompanyContractRequest> InboxContractRequests { get; set; } = new List<CompanyContractRequest>();
        public List<CompanyStorage> Storages { get; set; } = new List<CompanyStorage>();
        public List<ShowcaseStorage> Showcases { get; set; } = new List<ShowcaseStorage>();
    }
}
