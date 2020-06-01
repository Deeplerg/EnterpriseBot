using EnterpriseBot.Api.Models.Common.Enums;
using EnterpriseBot.Api.Models.Common.Storages;
using Newtonsoft.Json;

namespace EnterpriseBot.Api.Models.Common.Business
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

        [JsonIgnore]
        public string UnloadTruckJobId { get; set; }
        [JsonIgnore]
        public string ReturnTruckJobId { get; set; }
    }
}
