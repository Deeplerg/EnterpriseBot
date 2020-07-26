using System.Collections.Generic;

namespace EnterpriseBot.ApiWrapper.Models.Game.Business.Company
{
    public class TruckGarage
    {
        public long Id { get; set; }

        public Company Company { get; set; }

        public sbyte Capacity { get; set; }

        public List<Truck> Trucks { get; set; } = new List<Truck>();
    }
}
