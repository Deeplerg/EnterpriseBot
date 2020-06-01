using System.Collections.Generic;

namespace EnterpriseBot.ApiWrapper.Models.Common.Business
{
    public class TruckGarage
    {
        public long Id { get; set; }

        public long CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public byte Capacity { get; set; }
        public virtual List<Truck> Trucks { get; set; }
    }
}
