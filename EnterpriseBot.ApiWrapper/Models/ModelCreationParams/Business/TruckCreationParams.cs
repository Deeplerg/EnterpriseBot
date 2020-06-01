using EnterpriseBot.ApiWrapper.Models.Common.Storages;

namespace EnterpriseBot.ApiWrapper.Models.ModelCreationParams.Business
{
    public class TruckCreationParams
    {
        public long TruckGarageId { get; set; }

        /// <summary>
        /// <see cref="TrunkStorage"/> capacity. <br/>
        /// If null, default value will be assigned.
        /// </summary>
        public int? Capacity { get; set; }

        /// <summary>
        /// If null, default value will be assigned.
        /// </summary>
        public ushort? DeliveringSpeedInSeconds { get; set; }
    }
}
