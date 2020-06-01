using EnterpriseBot.ApiWrapper.Models.Common.Enums;
using EnterpriseBot.ApiWrapper.Models.Common.Storages;

namespace EnterpriseBot.ApiWrapper.Models.Common.Business
{
    public class Truck
    {
        public long Id { get; set; }

        public long TruckGarageId { get; set; }
        public virtual TruckGarage TruckGarage { get; set; }

        public long TrunkStorageId { get; set; }
        public virtual TrunkStorage TrunkStorage { get; set; }

        public ushort DeliveringSpeedInSeconds { get; set; }

        public TruckState CurrentState { get; set; }
    }
}
