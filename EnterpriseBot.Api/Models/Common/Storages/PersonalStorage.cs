using EnterpriseBot.Api.Models.Common.Essences;
using System.Collections.Generic;

namespace EnterpriseBot.Api.Models.Common.Storages
{
    public class PersonalStorage
    {
        public long Id { get; set; }

        //public int CellsAmount { get; set; }
        //public int CellsCapacity { get; set; }
        public int Capacity { get; set; }

        public long PlayerId { get; set; }
        public virtual Player Player { get; set; }


        public virtual List<StorageItem> Items { get; set; }
    }
}
