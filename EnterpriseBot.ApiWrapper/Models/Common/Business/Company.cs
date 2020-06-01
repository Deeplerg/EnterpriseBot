using EnterpriseBot.ApiWrapper.Models.Common.Crafting;
using EnterpriseBot.ApiWrapper.Models.Common.Essences;
using EnterpriseBot.ApiWrapper.Models.Common.Storages;
using System.Collections.Generic;

namespace EnterpriseBot.ApiWrapper.Models.Common.Business
{
    public class Company
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


        public long GeneralManagerId { get; set; }
        public virtual Player GeneralManager { get; set; }

        public decimal CompanyUnits { get; set; }


        public virtual List<Job> Jobs { get; set; }
        public virtual List<CandidateForJob> Candidates { get; set; }

        public virtual IncomeStorage IncomeStorage { get; set; }

        public virtual WorkerStorage WorkerStorage { get; set; }

        public virtual OutcomeStorage OutcomeStorage { get; set; }


        public virtual List<Contract> IncomeContracts { get; set; }
        public virtual List<Contract> OutcomeContracts { get; set; }

        public virtual List<ContractRequest> ContractRequests { get; set; }


        public virtual TruckGarage TruckGarage { get; set; }


        public virtual List<Item> OutputItems { get; set; }
    }
}
