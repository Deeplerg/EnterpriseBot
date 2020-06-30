using EnterpriseBot.Api.Game.Business.Company;
using EnterpriseBot.Api.Game.Storages;

namespace EnterpriseBot.Api.Models.ModelCreationParams.Business
{
    public class TruckCreationParams
    {
        public TruckGarage TruckGarage { get; set; }

        /// <summary>
        /// <see cref="TrunkStorage"/> capacity
        /// </summary>
        public int Capacity { get; set; }

        public uint DeliveringSpeedInSeconds { get; set; }
    }
}
