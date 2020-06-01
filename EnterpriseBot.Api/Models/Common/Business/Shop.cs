using EnterpriseBot.Api.Models.Common.Essences;
using EnterpriseBot.Api.Models.Common.Storages;
using System.Collections.Generic;

namespace EnterpriseBot.Api.Models.Common.Business
{
    public class Shop
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public decimal ShopUnits { get; set; }

        public long GeneralManagerId { get; set; }
        public virtual Player GeneralManager { get; set; }


        public virtual IncomeStorage IncomeStorage { get; set; }

        public virtual ShowcaseStorage ShowcaseStorage { get; set; }


        public virtual List<Contract> IncomeContracts { get; set; }

        public virtual List<ContractRequest> ContractRequests { get; set; }
    }
}
