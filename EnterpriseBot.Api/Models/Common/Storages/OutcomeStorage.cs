using EnterpriseBot.Api.Models.Common.Business;
using System.Collections.Generic;

namespace EnterpriseBot.Api.Models.Common.Storages
{
    public class OutcomeStorage
    {
        public long Id { get; set; }

        //public int CellsAmount { get; set; }
        //public int CellsCapacity { get; set; }
        public int Capacity { get; set; }

        public long OwningCompanyId { get; set; }
        public virtual Company OwningCompany { get; set; }


        public virtual List<StorageItem> Items { get; set; }
    }
}
