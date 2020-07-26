using EnterpriseBot.ApiWrapper.Models.Enums;
using EnterpriseBot.ApiWrapper.Models.Game.Storages;

namespace EnterpriseBot.ApiWrapper.Models.Game.Business.Company
{
    public class Truck
    {
        public long Id { get; set; }

        public TruckGarage TruckGarage { get; set; }

        public TrunkStorage Trunk { get; set; }

        public uint DeliveringSpeedInSeconds { get; set; }

        public TruckState CurrentState { get; set; }
    }
}
